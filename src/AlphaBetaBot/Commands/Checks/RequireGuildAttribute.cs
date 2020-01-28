using System.Threading.Tasks;
using Qmmands;

namespace AlphaBetaBot
{
    public class RequireGuildAttribute : AbfCheckBaseAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
                return new CheckResult("Invalid command context.");

            if (ctx.Guild is null)
                return new CheckResult("This command can only be used in a Discord Guild.");

            return CheckResult.Successful;
        }
    }
}
