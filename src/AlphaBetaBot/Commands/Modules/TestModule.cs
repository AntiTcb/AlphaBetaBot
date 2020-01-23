using System.Threading.Tasks;
using Qmmands;

namespace AlphaBetaBot.Commands
{
    public class TestModule : AbfModuleBase
    {
        [Command("test")]
        public async Task TestAsync()
        {
            await ReplyAsync("Test succeeded.");
        }
    }
}
