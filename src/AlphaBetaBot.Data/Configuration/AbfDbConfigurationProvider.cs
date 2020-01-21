using Microsoft.Extensions.Options;

namespace AlphaBetaBot.Data
{
    public class AbfDbConfigurationProvider
    {
        public AbfDbConfiguration Configuration { get; set; }
        public AbfDbConfigurationProvider(IOptions<AbfDbConfiguration> config)
        {
            Configuration = config.Value;
        }
    }
}
