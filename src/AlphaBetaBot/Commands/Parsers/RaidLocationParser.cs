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

            var result = value.ToLower() switch
            {
                "bwl" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),
                "blackwing lair" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),
                "blackwing" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),
                "nefarian" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),
                "nef" => new TypeParserResult<RaidLocationId>(RaidLocationId.BlackwingLair),

                "molten core" => new TypeParserResult<RaidLocationId>(RaidLocationId.MoltenCore),
                "mc" => new TypeParserResult<RaidLocationId>(RaidLocationId.MoltenCore),
                "rag" => new TypeParserResult<RaidLocationId>(RaidLocationId.MoltenCore),

                "naxxramas" => new TypeParserResult<RaidLocationId>(RaidLocationId.Naxxramas),
                "naxx" => new TypeParserResult<RaidLocationId>(RaidLocationId.Naxxramas),
                "nax" => new TypeParserResult<RaidLocationId>(RaidLocationId.Naxxramas),
                "kt" => new TypeParserResult<RaidLocationId>(RaidLocationId.Naxxramas),

                "onyxia's lair" => new TypeParserResult<RaidLocationId>(RaidLocationId.Onyxia),
                "onyxia" => new TypeParserResult<RaidLocationId>(RaidLocationId.Onyxia),
                "ony" => new TypeParserResult<RaidLocationId>(RaidLocationId.Onyxia),

                "zg" => new TypeParserResult<RaidLocationId>(RaidLocationId.ZulGurub),
                "zulgurub" => new TypeParserResult<RaidLocationId>(RaidLocationId.ZulGurub),
                "zul gurub" => new TypeParserResult<RaidLocationId>(RaidLocationId.ZulGurub),
                "zul'gurub" => new TypeParserResult<RaidLocationId>(RaidLocationId.ZulGurub),

                "ruins" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ20),
                "ruins of Ahn'Qiraj" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ20),
                "aq20" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ20),

                "temple" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ40),
                "temple of Ahn'Qiraj" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ40),
                "aq40" => new TypeParserResult<RaidLocationId>(RaidLocationId.AQ40),

                _ => new TypeParserResult<RaidLocationId>("Couldn't find matching raid.")
            };

            return result;
        }   
    }
}
