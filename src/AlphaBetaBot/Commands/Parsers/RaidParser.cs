using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Qmmands;

namespace AlphaBetaBot
{
    public class RaidParser : TypeParser<Raid>
    {
        public static readonly RaidParser Instance = new RaidParser();

        public override async ValueTask<TypeParserResult<Raid>> ParseAsync(Parameter parameter, string value, CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
            {
                return new TypeParserResult<Raid>("Invalid context type.");
            }

            if (!ulong.TryParse(value, out ulong raidId))
                return new TypeParserResult<Raid>("Couldn't parse to a message/raid ID.");

            var raid = await ctx.DatabaseContext.GetRaidAsync(raidId);

            if (raid is null)
                return new TypeParserResult<Raid>("Couldn't find a raid by that ID.");

            return new TypeParserResult<Raid>(raid);
        }
    }
}
