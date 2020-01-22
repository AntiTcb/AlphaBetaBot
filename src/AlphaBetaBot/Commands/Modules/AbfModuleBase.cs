using Disqord.Bot;

namespace AlphaBetaBot
{
    public class AbfModuleBase : DiscordModuleBase<AbfCommandContext>
    {
        public DatabaseCommandContext DbContext => Context.DatabaseContext;

    }
}
