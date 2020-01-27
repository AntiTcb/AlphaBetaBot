using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Extensions.Interactivity.Menus;
using Qmmands;

namespace AlphaBetaBot
{
    [Name("Wow")]
    public class WowModule : AbfModuleBase
    {
        [Name("Characters"), Group("character", "characters", "toons")]
        public class WowCharacterModule : AbfModuleBase
        {
            [Command("list", "")]
            public async Task ListCharactersAsync()
            {
                var characters = DbContext.User.Characters;

                var embed = new LocalEmbedBuilder()
                    .WithTitle($"{Context.User.Name}'s characters:");

                if (!characters.Any())
                {
                    embed.WithDescription("You have no characters currently.");
                    await ReplyAsync(embed: embed.Build());
                    return;
                }

                var sb = new StringBuilder();
                foreach (var character in characters)
                {
                    sb.AppendLine($"{character.CharacterName} | {character.Class} | {character.Role}");
                }
                embed.WithDescription(sb.ToString());
                await ReplyAsync(embed: embed.Build());
            }

            [Command("add")]
            public async Task AddCharacterAsync(string characterName, WowClass @class, ClassRole role)
            {
                var character = new WowCharacter { 
                    CharacterName = characterName, 
                    Class = @class, 
                    Role = role, 
                    Owner = DbContext.User 
                };

                DbContext.User.Characters.Add(character);
                await ReplyAsync($"Added character {character.CharacterName} | {character.Class} | {character.Role}");
            }

            [Command("remove", "delete")]
            public async Task RemoveCharacterAsync(string characterName)
            {
                var character = DbContext.User.Characters.FirstOrDefault(c => c.CharacterName == characterName);
                
                if (character is null)
                    await ReplyAsync("Character not found.");
                else {
                    DbContext.User.Characters.Remove(character);
                    await ReplyAsync($"{characterName} was removed from your character list.");
                }
            }
        }

        [Name("raid"), Group("raid", "raids")]
        [RequireOwner(Group = "perm")]
        [RequireUserPermissions(Permission.ManageGuild, Group = "perm")]
        public class WowRaidModule : AbfModuleBase
        {
            [Command("add", "create")]
            public async Task AddRaid(DateTimeOffset raidTime, RaidLocationId raidLocation)
            {
                if (DbContext.Guild.RaidSignupChannelId is null)
                {
                    await ReplyAsync("A raid signup channel needs to be setup. Use the settings signupchannel command to do so.");
                    return;
                }

                var raid = new Raid
                {
                    Id = Context.Message.Id,
                    RaidLocationId = raidLocation,
                    RaidTime = raidTime
                };

                await DbContext.AddRaidAsync(raid);
                await ReplyAsync("Raid added!");
            }

            [Command("signup")]
            public async Task SignupToRaidAsync(IUserMessage raidSignupMessage)
            {
                var raid = await DbContext.GetRaidAsync(raidSignupMessage.Id);
                var menu = new RaidSignupMenu(raid, DbContext);
                await Context.Channel.StartMenuAsync(menu);
            }

            public class RaidSignupMenu : MenuBase
            {
                private Raid _raid;
                private DatabaseCommandContext _dbContext;
                
                public RaidSignupMenu(Raid raid, DatabaseCommandContext dbContext) : base()
                {
                    _raid = raid;
                    _dbContext = dbContext;
                }
                protected override async Task<IUserMessage> InitialiseAsync()
                {
                    var message = await Channel.SendMessageAsync($"Signups for the {_raid.RaidLocationId} raid at {_raid.RaidTime} have started! Please use the below buttons to sign up your character to the raid.");
                    return message;
                }

                [Button("")]
                public async Task SignupWarriorAsync(ButtonEventArgs e)
                {
                    if (e.WasAdded)
                    {
                        var user = await e.User.Downloadable.GetOrDownloadAsync();
                    }
                    else
                    {

                    }
                }
            }
        }

        [Name("Settings"), Group("settings")]
        public class WowSettingsModule : AbfModuleBase
        {
            [Command("signupchannel")]
            public async Task SignupChannelAsync(IMessageChannel channel = null)
            {
                ulong? channelId = DbContext.Guild.RaidSignupChannelId;

                if (channel is null && channelId is null)
                {
                    await ReplyAsync("No raid signup channel exists.");
                }
                else if (channel is null)
                {
                    channel = Context.Guild.GetChannel(channelId.Value) as CachedTextChannel;
                    await ReplyAsync($"The raid sigup channel is {channel}");
                }
                else
                {
                    DbContext.Guild.RaidSignupChannelId = channel.Id;
                    await ReplyAsync($"The raid signup channel is now set to {channel}.");
                }
            }
        }


    }
}
