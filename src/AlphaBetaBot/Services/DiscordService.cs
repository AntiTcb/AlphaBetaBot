using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Prefixes;
using Disqord.Events;
using Disqord.Logging;
using Disqord.Rest;
using Humanizer;
using Qmmands;

namespace AlphaBetaBot
{
    public class DiscordService : DiscordBot
    {
        private readonly LogService _logger;
        private readonly AbfConfiguration _configuration;

        public DiscordService(AbfConfigurationProvider configProvider, IPrefixProvider prefixProvider, DiscordBotConfiguration discordConfig = null) 
            : base(TokenType.Bot, configProvider.Configuration.DiscordToken, prefixProvider, discordConfig)
        {
            _logger = LogService.GetLogger("Discord");
            _configuration = configProvider.Configuration;
        }

        public async Task SetupAsync(Assembly assembly)
        {
            Ready += OnReadyAsync;
            CommandExecutionFailed += DiscordService_CommandExecutionFailed;
            Logger.MessageLogged += OnMessageLogged;

            AddModules(assembly);
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
                case CommandNotFoundResult _: //this is handled at the beginning of this method with levenshtein thing.
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

        private Task DiscordService_CommandExecutionFailed(CommandExecutionFailedEventArgs e)
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
    }
}
