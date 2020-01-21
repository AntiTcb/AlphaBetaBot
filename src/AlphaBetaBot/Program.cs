using System.Threading.Tasks;
using Disqord.Bot.Prefixes;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaBetaBot
{
    class Program
    {
        static async Task Main()
        {
            var services = new ServiceCollection()
                .AddSingleton(new DefaultPrefixProvider().AddPrefix('!').AddMentionPrefix())
                .AddSingleton<AlphaBetaBot>();
        }
    }
}
