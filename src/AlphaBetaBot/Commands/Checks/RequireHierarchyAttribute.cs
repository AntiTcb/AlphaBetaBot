using System.Threading.Tasks;
using Disqord;
using Qmmands;

namespace AlphaBetaBot
{
    public sealed class RequireHierarchyAttribute : ParameterCheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(object argument, CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
            {
                return CheckResult.Unsuccessful("Invalid command context.");
            }

            if (!(argument is CachedMember mbr))
            {
                return CheckResult.Unsuccessful("The argument was not a CachedMember");
            }

            return ctx.Member.Hierarchy > mbr.Hierarchy && ctx.Guild.CurrentMember.Hierarchy > mbr.Hierarchy
                ? CheckResult.Successful
                : CheckResult.Unsuccessful($"Sorry. {mbr.ToString()} is protected.");
        }
    }
}
