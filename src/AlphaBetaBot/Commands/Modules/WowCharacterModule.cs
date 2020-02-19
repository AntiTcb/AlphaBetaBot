﻿using System;
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
                sb.AppendLine($"{character.CharacterName} | {character.Class} | {character.Role}");

            embed.WithDescription(sb.ToString());
            await ReplyAsync(embed: embed.Build());
        }

        [Command("add")]
        public Task AddCharacterAsync(string characterName, ClassRole role, WowClass @class) => AddCharacterAsync(characterName, @class, role);

        [Command("add")]
        public async Task AddCharacterAsync(string characterName, WowClass @class, ClassRole role)
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

        [Command("edit")]
        public async Task EditCharacterAsync(WowCharacter character, ClassRole role)
        {
            character.Role = role;

            await ReplyAsync($"Set {character.CharacterName}'s role to {role}");
        }

        [Command("remove", "delete")]
        public async Task RemoveCharacterAsync(WowCharacter character)
        {                
            if (character is null)
                await ReplyAsync("Character not found.");
            else {
                DbContext.Database.Remove(character);
                await ReplyAsync($"{character.CharacterName} was removed from your character list.");
            }
        }
    }
}
