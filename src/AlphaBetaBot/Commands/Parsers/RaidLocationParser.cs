using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Qmmands;

namespace AlphaBetaBot
{
    public class RaidLocationParser : TypeParser<RaidLocationId>
    {
        public static readonly RaidLocationParser Instance = new RaidLocationParser();
        public override ValueTask<TypeParserResult<RaidLocationId>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
            {
                return new TypeParserResult<RaidLocationId>("Invalid context type.");
            }

            var result = value switch
            {
                "BWL" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),
                "Blackwing Lair" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),
                "Blackwing" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),
                "Nefarian" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),

                "Molten Core" => new TypeParserResult<RaidLocationId>(RaidLocationId.MoltenCore),
                "MC" => new TypeParserResult<RaidLocationId>(RaidLocationId.MoltenCore),

                "Naxxramas" => new TypeParserResult<RaidLocationId>(RaidLocationId.Naxxramas),
                "Naxx" => new TypeParserResult<RaidLocationId>(RaidLocationId.Naxxramas),

                "Onyxia's Lair" => new TypeParserResult<RaidLocationId>(RaidLocationId.Onyxia),
                "Onyxia" => new TypeParserResult<RaidLocationId>(RaidLocationId.Onyxia),
                "Ony" => new TypeParserResult<RaidLocationId>(RaidLocationId.Onyxia),

                "ZG" => new TypeParserResult<RaidLocationId>(RaidLocationId.ZulGurub),
                "Zul Gurub" => new TypeParserResult<RaidLocationId>(RaidLocationId.ZulGurub),
                "Zul'Gurub" => new TypeParserResult<RaidLocationId>(RaidLocationId.ZulGurub),

                "Ruins" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ20),
                "Ruins of Ahn'Qiraj" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ20),
                "AQ20" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ20),

                "Temple" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ40),
                "Temple of Ahn'Qiraj" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ40),
                "AQ40" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ40),

                _ => new TypeParserResult<RaidLocationId>("Couldn't find matching raid.")
            };

            return result;
        }   
    }
}
