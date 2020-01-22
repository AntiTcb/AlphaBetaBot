using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Qmmands;

namespace AlphaBetaBot
{
    [Name("Wow")]
    public class WowModule : AbfModuleBase
    {
        [Name("Characters"), Group("character")]
        public class WowCharacterModule : AbfModuleBase
        {
            [Command("list")]
            public async Task ListCharactersAsync()
            {
                var characters = DbContext.User.Characters.ToArray();

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
                await ReplyAsync(embed: embed.Build());
            }

            [Command("add")]
            public async Task AddCharacterAsync(string characterName, WowClass @class, ClassRole role)
            {
                var repo = DbContext.Database.RequestRepository<WowCharacterRepository>();
                var character = await repo.AddAsync(new WowCharacter { CharacterName = characterName, Class = @class, Role = role, Owner = DbContext.User });
                await ReplyAsync($"Added character {character.CharacterName} | {character.Class} | {character.Role}");
            }
        }
    }
}
