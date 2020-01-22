using Microsoft.Extensions.Options;

namespace AlphaBetaBot.Data
{
    public class DatabaseConfigurationProvider : IDatabaseConfigurationProvider
    {
        private readonly DatabaseConfiguration _configuration;
        public DatabaseConfigurationProvider(IOptions<DatabaseConfiguration> config) => _configuration = config.Value;
        public DatabaseConfiguration GetConfiguration() => _configuration;
    }

    public interface IDatabaseConfigurationProvider
    {
        DatabaseConfiguration GetConfiguration();
    }
}
