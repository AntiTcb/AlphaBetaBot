using System.IO;
using Microsoft.EntityFrameworkCore.Design;
using Newtonsoft.Json.Linq;

namespace AlphaBetaBot.Data
{
    public sealed class DesignTimeAbfContextFactory : IDesignTimeDbContextFactory<AbfDbContext>
    {
        public AbfDbContext CreateDbContext(string[] args)
        {
            return new AbfDbContext(new ConnectionStringProvider(new DesignTimeDatabaseConfigurationProvider()));
        }
    }

    public sealed class DesignTimeDatabaseConfigurationProvider : IDatabaseConfigurationProvider
    {

        public DatabaseConfiguration GetConfiguration()
        {
            var config = JObject.Parse(File.ReadAllText("../AlphaBetaBot/appsettings.json"))["Database"];

            return new DatabaseConfiguration
            {
                Host = config["Host"].Value<string>(),
                Port = config["Port"].Value<int>(),
                Database = config["Database"].Value<string>(),
                Username = config["Username"].Value<string>(),
                Password = config["Password"].Value<string>(),
            };
        }
    }
}
