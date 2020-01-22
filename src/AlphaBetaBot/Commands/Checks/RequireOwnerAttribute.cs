using System.Threading.Tasks;
using Qmmands;

namespace AlphaBetaBot
{
    public sealed class RequireOwnerAttribute : AbfCheckBaseAttribute
    {
        public override string Name { get; set; } = "Owner command";

        public override async ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            if (!(context is AbfCommandContext ctx))
            {
                return CheckResult.Unsuccessful("Invalid command context.");
            }

            var application = await ctx.Bot.GetCurrentApplicationAsync();
            return application.Owner.Id == ctx.User.Id
                ? CheckResult.Successful
                : CheckResult.Unsuccessful("You need to be owner of the bot for this.");
        }
    }
}
