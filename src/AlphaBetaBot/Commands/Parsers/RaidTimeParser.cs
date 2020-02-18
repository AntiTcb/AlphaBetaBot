using System.Threading.Tasks;
using Qmmands;

namespace AlphaBetaBot
{
    public class RaidTimeParser : TypeParser<RaidTime>
    {
        public static readonly RaidTimeParser Instance = new RaidTimeParser();
        public override ValueTask<TypeParserResult<RaidTime>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
            {
                return new TypeParserResult<RaidTime>("Invalid context type.");
            }

            bool isRaidTime = RaidTime.TryParse(value, out var rt);

            if (!isRaidTime)
                return new TypeParserResult<RaidTime>("Couldn't parse to raid time."); ;

            return new TypeParserResult<RaidTime>(rt);
        }
    }
}
