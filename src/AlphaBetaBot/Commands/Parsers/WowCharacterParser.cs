using System;
using System.Linq;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Qmmands;

namespace AlphaBetaBot
{
    public class WowCharacterParser : TypeParser<WowCharacter>
    {
        public static readonly WowCharacterParser Instance = new WowCharacterParser();

        public override ValueTask<TypeParserResult<WowCharacter>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
            {
                return new TypeParserResult<WowCharacter>("Invalid context type.");
            }

            var character = ctx.DatabaseContext.User.Characters.FirstOrDefault(c => string.Equals(c.CharacterName, value, StringComparison.OrdinalIgnoreCase));

            if (character is null)
                return new TypeParserResult<WowCharacter>("Character not found.");

            return new TypeParserResult<WowCharacter>(character);
        }
    }
}
