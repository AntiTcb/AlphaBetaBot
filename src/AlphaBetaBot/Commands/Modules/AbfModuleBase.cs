using System.Threading.Tasks;
using Disqord;
using Disqord.Bot;

namespace AlphaBetaBot
{
    public class AbfModuleBase : DiscordModuleBase<AbfCommandContext>
    {
        public DatabaseCommandContext DbContext => Context.DatabaseContext;

        public Task ConfirmAsync() => Context.Message.AddReactionAsync(new LocalEmoji("✅"));
    }
}
