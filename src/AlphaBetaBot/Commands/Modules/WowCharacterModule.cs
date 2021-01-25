using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Extensions.Interactivity;
using Qmmands;

namespace AlphaBetaBot
{
    [Name("Characters"), Group("character", "characters", "toons")]
    public class WowCharacterModule : AbfModuleBase
    {
        [Command("list")]
        [Description("Lists off all the characters you've registered with the bot.")]
        [Remarks("!characters list")]
        public async Task ListCharactersAsync()
        {
            var characters = DbContext.User.Characters;

            var embed = new LocalEmbedBuilder()
                .WithTitle($"{Context.Member.DisplayName}'s characters:");

            if (!characters.Any())
            {
                embed.WithDescription("You have no characters registered. You may do with `!character add NameOfYourToon ClassName Role (Melee|Ranged|Tank|Healer)`");
                await ReplyAsync(embed: embed.Build());
                return;
            }

            var sb = new StringBuilder();
            foreach (var character in characters)
                sb.AppendLine($"{character.CharacterName} | {character.Class} | {character.Role}");

            embed.WithDescription(sb.ToString());
            await ReplyAsync(embed: embed.Build());
        }

        [Command("list"), Hidden]
        [Description("Lists off all the characters the mentioned person has registered with the bot.")]
        public async Task ListCharactersAsync(CachedMember user)
        {
            var characters = DbContext.Database.Users.FirstOrDefault(u => u.Id == user.Id)?.Characters;

            var embed = new LocalEmbedBuilder()
                .WithTitle($"{user.DisplayName}'s characters:");

            if (characters is null || !characters.Any())
            {
                embed.WithDescription($"They have no characters.");
                await ReplyAsync(embed: embed.Build());
                return;
            }

            var sb = new StringBuilder();
            foreach (var character in characters)
                sb.AppendLine($"{character.CharacterName} | {character.Class} | {character.Role}");

            embed.WithDescription(sb.ToString());
            await ReplyAsync(embed: embed.Build());
        }

        [Command("add"), RunMode(RunMode.Parallel)]
        [Description("Adds a character to the bot. This is an interactive version, where the bot will ask you questions you'll need to reply to.")]
        public async Task InteractiveAddCharacterAsync()
        {
            var interactivity = Context.Channel.Client.GetExtension<InteractivityExtension>();

            var msg = await ReplyAsync("What is your character's name?");
            var characterNameReply = await interactivity.WaitForMessageAsync(e => e.Message.Channel.Id == Context.Channel.Id && e.Message.Author.Id == Context.User.Id, TimeSpan.FromSeconds(15));

            if (characterNameReply is null)
            {
                await msg.ModifyAsync(m => m.Content = "I don't have all day. Try the command again, and reply within 15 seconds!");
                return;
            }
            string characterName = characterNameReply.Message.Content;
            var messagesToDelete = new List<IMessage>() { Context.Message, characterNameReply.Message };

            var msg2 = await ReplyAsync($"What class is {characterName}? (Druid, Hunter, Mage, Paladin, Priest, Rogue, Warlock, Warrior)");
            var classReply = await interactivity.WaitForMessageAsync(e => e.Message.Channel.Id == Context.Channel.Id && e.Message.Author.Id == Context.User.Id, TimeSpan.FromSeconds(15));

            if (classReply is null)
            {
                await msg.ModifyAsync(m => m.Content = "I don't have all day. Try the command again, and reply within 15 seconds!");
                await (Context.Channel as CachedTextChannel).DeleteMessagesAsync(messagesToDelete.Select(m => m.Id));
                return;
            }
            var @class = Enum.Parse<WowClass>(classReply.Message.Content, true);
            messagesToDelete.Add(msg2);
            messagesToDelete.Add(classReply.Message);

            ClassRole role;
            if (@class == WowClass.Mage || @class == WowClass.Warlock || @class == WowClass.Hunter)
            {
                role = ClassRole.Ranged;
            }
            else if (@class == WowClass.Rogue)
            {
                role = ClassRole.Melee;
            }
            else
            {
                var msg3 = await ReplyAsync($"What is role is {characterName}? (Melee, Ranged, Tank, or Healer)");
                var roleReply = await interactivity.WaitForMessageAsync(e => e.Message.Channel.Id == Context.Channel.Id && e.Message.Author.Id == Context.User.Id, TimeSpan.FromSeconds(15));

                if (roleReply is null)
                {
                    await msg.ModifyAsync(m => m.Content = "I don't have all day. Try the command again, and reply within 15 seconds!");
                    await (Context.Channel as CachedTextChannel).DeleteMessagesAsync(messagesToDelete.Select(m => m.Id));
                    return;
                }

                role = Enum.Parse<ClassRole>(roleReply.Message.Content, true);
                messagesToDelete.Add(msg3);
                messagesToDelete.Add(roleReply.Message);
            }

            await AddCharacterAsync(characterName, @class, role);
            await DbContext.Database.SaveChangesAsync();

            await (Context.Channel as CachedTextChannel).DeleteMessagesAsync(messagesToDelete.Select(m => m.Id));

            var finalMsg = await ReplyAsync($"You've successfully registered {characterName} to your characters list. You may now sign up for raids with {characterName} by clicking the {@class} icon on the raid signup message.");

            _ = Task.Run(async () => { await Task.Delay(TimeSpan.FromSeconds(15)); await finalMsg.DeleteAsync(); });
        }

        [Command("add"), RunMode(RunMode.Parallel), Hidden]
        [Description("Adds a character to the bot, for the person mentioned.")]
        public async Task InteractiveAddCharacterAsync(CachedMember user)
        {
            var interactivity = Context.Channel.Client.GetExtension<InteractivityExtension>();

            var msg = await ReplyAsync($"{user.Mention} What is your character's name?");
            var characterNameReply = await interactivity.WaitForMessageAsync(e => e.Message.Channel.Id == Context.Channel.Id && e.Message.Author.Id == user.Id, TimeSpan.FromSeconds(15));

            if (characterNameReply is null)
            {
                await msg.ModifyAsync(m => m.Content = "I don't have all day. Try the command again, and reply within 15 seconds!");
                return;
            }
            string characterName = characterNameReply.Message.Content;

            var messagesToDelete = new List<IMessage>() { Context.Message, characterNameReply.Message };

            var msg2 = await ReplyAsync($"What class is {characterName}? (Druid, Hunter, Mage, Paladin, Priest, Rogue, Warlock, Warrior)");
            var classReply = await interactivity.WaitForMessageAsync(e => e.Message.Channel.Id == Context.Channel.Id && e.Message.Author.Id == user.Id, TimeSpan.FromSeconds(15));

            if (classReply is null)
            {
                await msg.ModifyAsync(m => m.Content = "I don't have all day. Try the command again, and reply within 15 seconds!");
                await (Context.Channel as CachedTextChannel).DeleteMessagesAsync(messagesToDelete.Select(m => m.Id));
                return;
            }
            var @class = Enum.Parse<WowClass>(classReply.Message.Content, true);
            messagesToDelete.Add(msg2);
            messagesToDelete.Add(classReply.Message);

            ClassRole role;
            if (@class == WowClass.Mage || @class == WowClass.Warlock || @class == WowClass.Hunter)
            {
                role = ClassRole.Ranged;
            }
            else if (@class == WowClass.Rogue)
            {
                role = ClassRole.Melee;
            }
            else
            {
                var msg3 = await ReplyAsync($"What is role is {characterName}? (Melee, Ranged, Tank, or Healer)");
                var roleReply = await interactivity.WaitForMessageAsync(e => e.Message.Channel.Id == Context.Channel.Id && e.Message.Author.Id == user.Id, TimeSpan.FromSeconds(15));

                if (roleReply is null)
                {
                    await msg.ModifyAsync(m => m.Content = "I don't have all day. Try the command again, and reply within 15 seconds!");
                    await (Context.Channel as CachedTextChannel).DeleteMessagesAsync(messagesToDelete.Select(m => m.Id));
                    return;
                }

                role = Enum.Parse<ClassRole>(roleReply.Message.Content, true);
                messagesToDelete.Add(msg3);
                messagesToDelete.Add(roleReply.Message);
            }

            await AddCharacterAsync(user, characterName, @class, role);
            await DbContext.Database.SaveChangesAsync();

            await (Context.Channel as CachedTextChannel).DeleteMessagesAsync(messagesToDelete.Select(m => m.Id));

            var finalMsg = await ReplyAsync($"You've successfully registered {characterName} to your characters list. You may now sign up for raids with {characterName} by clicking the {@class} icon on the raid signup message.");

            _ = Task.Run(async () => { await Task.Delay(TimeSpan.FromSeconds(15)); await finalMsg.DeleteAsync(); });
        }

        [Command("add")]
        [Description("Adds a character to the bot. You can put the role and class in either order.")]
        [Remarks("!characters add Gnomeorpuns Ranged Mage")]
        public Task AddCharacterAsync([Description("Character Name")] string characterName, [Description("Role: Tank|Healer|Melee|Ranged")] ClassRole role, [Description("Class: Druid|Hunter|Mage|Paladin|Priest|Rogue|Warlock|Warrior")] WowClass @class)
            => AddCharacterAsync(characterName, @class, role);

        [Command("add"), Hidden]
        [Description("Adds a character to the bot.")]
        [Remarks("!characters add Gnomeorpuns Mage Ranged")]
        public async Task AddCharacterAsync([Description("Character Name")] string characterName, [Description("Class: Druid|Hunter|Mage|Paladin|Priest|Rogue|Warlock|Warrior")] WowClass @class, [Description("Role: Tank|Healer|Melee|Ranged")] ClassRole role)
        {
            if (DbContext.User.Characters.Any(c => c.Class == @class))
            {
                var msg = await ReplyAsync("You can only add one character per class.");
                _ = Task.Run(async () => { await Task.Delay(TimeSpan.FromSeconds(15)); await msg.DeleteAsync(); });
                return;
            }

            var character = new WowCharacter
            {
                CharacterName = characterName,
                Class = @class,
                Role = role,
                Owner = DbContext.User
            };

            DbContext.User.Characters.Add(character);
            await ConfirmAsync();
        }

        [Command("add"), Hidden]
        [Description("Adds a character to the bot, for the person mentioned.")]
        [Remarks("!characters add @Gnomeorpuns Gnomeorpuns Mage Ranged")]
        public async Task AddCharacterAsync([Description("Mention or username")] CachedMember user, [Description("Character Name")] string characterName, [Description("Class: Druid|Hunter|Mage|Paladin|Priest|Rogue|Warlock|Warrior")] WowClass @class, [Description("Role: Tank|Healer|Melee|Ranged")] ClassRole role)
        {
            var dbUser = await DbContext.Database.RequestRepository<UserRepository>().GetOrAddAsync(user.Id);

            var character = new WowCharacter
            {
                CharacterName = characterName,
                Class = @class,
                Role = role,
                Owner = dbUser
            };

            dbUser.Characters.Add(character);

            await DbContext.Database.SaveChangesAsync();
            await ConfirmAsync();
        }

        [Command("add"), Hidden]
        [Description("Adds a character to the bot.")]
        [Remarks("!characters add @Gnomeorpuns Gnomeorpuns Mage Ranged")]
        public Task AddCharacterAsync([Description("Mention or username")] CachedMember user, [Description("Character Name")] string characterName, [Description("Role: Tank|Healer|Melee|Ranged")] ClassRole role, [Description("Class: Druid|Hunter|Mage|Paladin|Priest|Rogue|Warlock|Warrior")] WowClass @class)
            => AddCharacterAsync(user, characterName, @class, role);

        [Command("changerole")]
        [Description("Change the role of the specified character.")]
        [Remarks("!characters Youther Healer")]
        public async Task EditCharacterAsync([Description("Character Name")] WowCharacter character, [Description("Role: Tank|Healer|Melee|Ranged")] ClassRole role)
        {
            character.Role = role;

            await ReplyAsync($"Set {character.CharacterName}'s role to {role}");
        }

        [Command("remove", "delete")]
        [Description("Removes the specified character from your character list.")]
        [Remarks("!characters remove Perishable")]
        public async Task RemoveCharacterAsync([Description("Character Name")] WowCharacter character)
        {
            if (character is null)
            {
                await ReplyAsync("Character not found.");
            }
            else
            {
                DbContext.Database.Remove(character);
                await ReplyAsync($"{character.CharacterName} was removed from your character list.");
            }
        }
    }
}
