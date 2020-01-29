using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Prefixes;
using Disqord.Events;
using Disqord.Logging;
using Disqord.Rest;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace AlphaBetaBot
{
    public class DiscordService : DiscordBot
    {
        private readonly LogService _logger;
        private readonly AbfConfiguration _configuration;

        public DiscordService(AbfConfigurationProvider configProvider, IPrefixProvider prefixProvider, IServiceProvider services, DiscordBotConfiguration discordConfig = null) 
            : base(TokenType.Bot, configProvider.Configuration.DiscordToken, prefixProvider, discordConfig)
        {
            _logger = LogService.GetLogger("Discord");
            _configuration = configProvider.Configuration;
        }

        public async Task SetupAsync(Assembly assembly)
        {
            Ready += OnReadyAsync;
            CommandExecutionFailed += HandleCommandExecutionFailedAsync;
            Logger.MessageLogged += OnMessageLogged;

            ReactionAdded += HandleRaidSignupAsync;
            ReactionRemoved += HandleRaidResignAsync;
            
            AddModules(assembly);
        }

        private async Task HandleRaidSignupAsync(ReactionAddedEventArgs e)
        {
            static string[] GetClassNames() => Enum.GetNames(typeof(WowClass));

            if (!GetClassNames().Contains(e.Emoji.Name)) return;

            var dbContext = this.GetRequiredService<AbfDbContext>();

            await using (dbContext)
            {
                var (check, raid, user, character) = await CheckForRaidAsync(e.Message.Id.RawValue, e.User.Id.RawValue, e.Emoji.Name, dbContext);
                if (!check) return;


                if (raid.Participants.Any(p => p.CharacterId == character.Id)) return;

                var participant = new RaidParticipant { Character = character, Raid = raid, SignedUpAt = DateTimeOffset.UtcNow };

                raid.Participants.Add(participant);
                dbContext.Update(raid);

                await dbContext.SaveChangesAsync();

                var msg = await e.Message.Downloadable.GetOrDownloadAsync() as RestUserMessage;
                await CreateRaidEmbedAsync(msg, raid);
            }
        }

        private async Task HandleRaidResignAsync(ReactionRemovedEventArgs e)
        {
            static string[] GetClassNames() => Enum.GetNames(typeof(WowClass));

            if (!GetClassNames().Contains(e.Emoji.Name)) return;

            var dbContext = this.GetRequiredService<AbfDbContext>();

            await using (dbContext)
            {
                var (check, raid, user, character) = await CheckForRaidAsync(e.Message.Id.RawValue, e.User.Id.RawValue, e.Emoji.Name, dbContext);
                if (!check) return;

                var charToRemove = raid.Participants.FirstOrDefault(p => p.CharacterId == character.Id);

                if (charToRemove is null) return;

                raid.Participants.Remove(charToRemove);
                dbContext.Update(raid);

                await dbContext.SaveChangesAsync();

                var msg = await e.Message.Downloadable.GetOrDownloadAsync() as RestUserMessage;
                await CreateRaidEmbedAsync(msg, raid);
            }
        }

        protected override async ValueTask AfterExecutedAsync(IResult result, DiscordCommandContext context)
        {
            var ctx = (AbfCommandContext)context;

            if (result.IsSuccessful)
            {
                await ctx.EndAsync();
                return;
            }

            await ctx.EndAsync();

            var str = new StringBuilder();

            switch (result)
            {
                case ChecksFailedResult err:
                    str.AppendLine("The following check(s) failed:");
                    foreach (var (check, error) in err.FailedChecks)
                    {
                        str.AppendLine($"[`{(check as AbfCheckBaseAttribute)?.Name ?? check.GetType().Name}`]: `{error}`");
                    }
                    break;
                case TypeParseFailedResult err:
                    str.AppendLine(err.Reason);
                    break;
                case ArgumentParseFailedResult _:
                    str.AppendLine($"The syntax of the command `{ctx.Command.FullAliases[0]}` was wrong.");
                    break;
                case OverloadsFailedResult err:
                    str.AppendLine($"I can't find any valid overload for the command `{ctx.Command.Name}`.");
                    foreach (var overload in err.FailedOverloads)
                    {
                        str.AppendLine($" -> `{overload.Value.Reason}`");
                    }
                    break;
                case ParameterChecksFailedResult err:
                    str.AppendLine("The following parameter check(s) failed:");
                    foreach (var (check, error) in err.FailedChecks)
                    {
                        str.AppendLine($"[`{check.Parameter.Name}`]: `{error}`");
                    }
                    break;
                case ExecutionFailedResult _: //this should be handled in the CommandErrored event or in the Custom Result case.
                    break;
                case CommandNotFoundResult err: //this is handled at the beginning of this method with levenshtein thing.
                    str.AppendLine($"Command Not Found: {err.Reason}");    
                    break;
                case CommandOnCooldownResult err:
                    var remainingTime = err.Cooldowns.OrderByDescending(x => x.RetryAfter).FirstOrDefault();
                    str.AppendLine($"You're being rate limited! Please retry after {remainingTime.RetryAfter.Humanize()}.");
                    break;
                default:
                    str.AppendLine($"Unknown error: {result}");
                    break;
            }

            if (str.Length == 0)
            {
                return;
            }

            var embed = new LocalEmbedBuilder
            {
                Title = "Something went wrong!"
            };

            embed.WithFooter($"Type '{ctx.Prefix}help {ctx.Command?.FullAliases[0] ?? ctx.Command?.FullAliases[0] ?? ""}' for more information.");

            embed.AddField("__Command/Module__", ctx.Command?.Name ?? ctx.Command?.Module?.Name ?? "Unknown command...", true);
            embed.AddField("__Author__", ctx.User.ToString(), true);
            embed.AddField("__Error(s)__", str.ToString());

            _logger.Warning($"{ctx.User.Id} - {ctx.Guild.Id} ::> Command errored: {ctx.Command?.Name ?? "-unknown command-"}");
            await ctx.Channel.SendMessageAsync("", false, embed.Build());

            await ctx.DisposeAsync();
        }

        protected override async ValueTask<DiscordCommandContext> GetCommandContextAsync(CachedUserMessage message,
            IPrefix prefix)
        {
            var ctx = new AbfCommandContext(this, message, prefix);
            await ctx.PrepareAsync();

            return ctx;
        }

        private Task HandleCommandExecutionFailedAsync(CommandExecutionFailedEventArgs e)
        {
            var ctx = (AbfCommandContext)e.Context;

            _logger.Error($"Command errored: {e.Context.Command.Name} by {ctx.User.Id} in {ctx.Guild.Id}", e.Result.Exception);

            var str = new StringBuilder();
            switch (e.Result.Exception)
            {
                case DiscordHttpException ex when ex.HttpStatusCode == HttpStatusCode.Unauthorized:
                    str.AppendLine("I don't have enough power to perform this action. (please check that the hierarchy of the bot is correct)");
                    break;
                case DiscordHttpException ex when ex.HttpStatusCode == HttpStatusCode.BadRequest:
                    str.AppendLine($"The requested action has been stopped by Discord: `{ex.Message}`");
                    break;
                case DiscordHttpException ex:
                    str.AppendLine($":angry: | Something bad happened: [{ex.HttpStatusCode}] {ex.Message}");
                    break;
                case ArgumentException ex:
                    str.AppendLine($"{ex.Message}\n");
                    str.AppendLine($"Are you sure you didn't fail when typing the command? Please do `{ctx.Prefix}help {e.Result.Command.FullAliases[0]}`");
                    break;
                default:
                    _logger.Error($"{e.Result.Exception.GetType()} occured.", e.Result.Exception);
                    break;
            }

            if (str.Length == 0)
            {
                return Task.CompletedTask;
            }

            var embed = new LocalEmbedBuilder
            {
                Title = "Something went wrong!"
            };

            embed.AddField("__Command__", e.Result.Command.Name, true);
            embed.AddField("__Author__", ctx.User.ToString(), true);
            embed.AddField("__Error(s)__", str.ToString());
            embed.WithFooter($"Type '{ctx.Prefix}help {ctx.Command.FullAliases[0].ToLowerInvariant()}' for more information.");

            return ctx.Channel.SendMessageAsync("", false, embed.Build());
        }

        private void OnMessageLogged(object sender, MessageLoggedEventArgs e) => _logger.Log(e.Severity, e.Message, e.Exception);

        private Task OnReadyAsync(ReadyEventArgs e)
        {
            _logger.Info("AlphaBetaBot is ready.");

            return e.Client.SetPresenceAsync(UserStatus.Online, new LocalActivity("Classic WoW", ActivityType.Playing));
        }

        private static async Task CreateRaidEmbedAsync(RestUserMessage msg, Raid raid)
        {
            var embed = new LocalEmbedBuilder().WithTitle($"Roster: {raid.Participants.Count}");

            var lines = raid.Participants.OrderBy(p => p.SignedUpAt).Select((p, i) => $"{i + 1}) {AbfConfiguration.ClassEmojis[p.Character.Class]} | {p.Character.CharacterName} | {p.Character.Role}");
            var roleCounts = raid.Participants.GroupBy(p => p.Character.Role).Select(g => (Role: g.Key, Count: g.Count()));
            var classCounts = raid.Participants.GroupBy(rp => rp.Character.Class).Select(g => (Class: g.Key, Count: g.Count()));

            embed.WithDescription(string.Join('\n', lines));

            foreach (var (Role, Count) in roleCounts)
                embed.AddField(Role.ToString(), Count);
        
            foreach (var (Class, Count) in classCounts)
                embed.AddField(Class.ToString(), Count, true);

            await msg.ModifyAsync(m => m.Embed = embed.Build());
        }

        private static async Task<(bool, Raid, User, WowCharacter)> CheckForRaidAsync(ulong messageId, ulong userId, string emoteName, AbfDbContext dbContext)
        {
            await dbContext.Raids.Include(r => r.Participants).ToListAsync();
            var raid = await dbContext.Raids.FirstOrDefaultAsync(r => r.Id == messageId);

            if (raid is null) return (false, null, null, null);

            await dbContext.Users.Include(u => u.Characters).ToListAsync();
            var user = await dbContext.Users.FindAsync(userId);

            if (user is null) return (false, raid, null, null);

            var character = user.Characters.FirstOrDefault(c => c.Class.Humanize() == emoteName);
            if (character is null) return (false, raid, user, null);

            return (true, raid, user, character);
        }
    }
}
