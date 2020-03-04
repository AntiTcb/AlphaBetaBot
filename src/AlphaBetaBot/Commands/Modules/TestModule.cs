using System;
using System.Linq;
using System.Threading.Tasks;
using Qmmands;

namespace AlphaBetaBot
{
    public class TestModule : AbfModuleBase
    {
        [Command("test")]
        public async Task TestAsync()
        {
            await ReplyAsync("Test succeeded.");
        }

        [Command("invite")]
        public async Task InviteAsync()
        {
            await ReplyAsync("https://discordapp.com/api/oauth2/authorize?client_id=668648526431125505&permissions=1074261185&scope=bot");
        }

        [Command("findtimezones")]
        public Task TzsAsync()
        {
            var tzs = string.Join("\n", TimeZoneInfo.GetSystemTimeZones().Select(tz => tz.Id));

            Console.WriteLine(tzs);

            return Task.CompletedTask;
        }
    }
}
