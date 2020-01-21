using Microsoft.Extensions.Options;

namespace AlphaBetaBot
{
    public class AbfConfigurationProvider
    {
        public AbfConfiguration Configuration { get; set; }
        public AbfConfigurationProvider(IOptions<AbfConfiguration> config)
        {
            Configuration = config.Value;
        }
    }
}
