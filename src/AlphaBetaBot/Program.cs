using System.Threading.Tasks;

namespace AlphaBetaBot
{
    class Program
    {
        static async Task Main()
        {
            var bot = new AlphaBetaBot();
            await bot.InitializeAsync();
        }
    }
}
