using Npgsql;

namespace AlphaBetaBot.Data
{
    public class ConnectionStringProvider
    {
        public string ConnectionString { get; }

        public ConnectionStringProvider(AbfDbConfigurationProvider databaseConfiguration)
        {
            var config = databaseConfiguration.Configuration;

            ConnectionString = new NpgsqlConnectionStringBuilder
            {
                Host = config.Host,
                Port = config.Port,
                Database = config.Database,
                Username = config.Username,
                Password = config.Password
            }.ConnectionString;
        }
    }
}
