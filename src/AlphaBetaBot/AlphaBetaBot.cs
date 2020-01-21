using System;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaBetaBot
{
    public class AlphaBetaBot 
    {
        private const string DEFAULT_CONFIG_PATH = "appsettings.json";
        private IConfigurationRoot _configuration;
        private IServiceProvider _services;
        
        private async Task InitializeAsync(string configPath = DEFAULT_CONFIG_PATH)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(configPath, false, true)
                .Build();

            _configuration = config;
            _services = BuildServiceProvider();

            try
            {

            }
            catch
            {

            }

            var ds = _services.GetRequiredService<DiscordService>();
        }

        private IServiceProvider BuildServiceProvider()
        {
            return new ServiceCollection()
                .Configure<AbfConfiguration>(x => _configuration.GetSection("Bot").Bind(x))
                .AddSingleton<AbfConfigurationProvider>()
                .Configure<AbfDbConfiguration>(x => _configuration.GetSection("Database").Bind(x))
                .BuildServiceProvider();
        }
    }
}
