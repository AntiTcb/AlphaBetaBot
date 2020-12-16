using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
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
                .WithTitle($"{Context.User.Name}'s characters:");

            if (!characters.Any())
            {
                embed.WithDescription("You have no characters currently.");
                await ReplyAsync(embed: embed.Build());
                return;
            }

            var sb = new StringBuilder();
            foreach (var character in characters)
                sb.AppendLine($"{character.CharacterName} | {character.Class} | {character.Role}");

            embed.WithDescription(sb.ToString());
            await ReplyAsync(embed: embed.Build());
        }

        [Command("list")]
        public async Task ListCharactersAsync(CachedUser user)
        {
            var characters = DbContext.Database.Users.FirstOrDefault(u => u.Id == user.Id)?.Characters;

            var embed = new LocalEmbedBuilder()
                .WithTitle($"{user.Name}'s characters:");

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

        [Command("add")]
        [Description("Adds a character to the bot.")]
        [Remarks("!characters add Gnomeorpuns Ranged Mage")]
        public Task AddCharacterAsync([Description("Character Name")] string characterName, [Description("Role: Tank|Healer|Melee|Ranged")] ClassRole role, [Description("Class: Druid|Hunter|Mage|Paladin|Priest|Warlock|Warrior")] WowClass @class) 
            => AddCharacterAsync(characterName, @class, role);

        [Command("add")]
        [Description("Adds a character to the bot.")]
        [Remarks("!characters add Gnomeorpuns Mage Ranged")]
        public async Task AddCharacterAsync([Description("Character Name")] string characterName, [Description("Class: Druid|Hunter|Mage|Paladin|Priest|Warlock|Warrior")] WowClass @class, [Description("Role: Tank|Healer|Melee|Ranged")] ClassRole role)
        {
            if (DbContext.User.Characters.Any(c => c.Class == @class))
            {
                await ReplyAsync("You can only add one character per class.");
                return;
            }
            
            var character = new WowCharacter { 
                CharacterName = characterName, 
                Class = @class, 
                Role = role, 
                Owner = DbContext.User 
            };

            DbContext.User.Characters.Add(character);
            await ConfirmAsync();
        }

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
            else {
                DbContext.Database.Remove(character);
                await ReplyAsync($"{character.CharacterName} was removed from your character list.");
            }
        }
    }
}
