using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Prefixes;
using Disqord.Extensions.Interactivity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;

namespace AlphaBetaBot
{
    public class AlphaBetaBot 
    {
        private const string DEFAULT_CONFIG_PATH = "appsettings.json";
        private IConfigurationRoot _configuration;
        private IServiceProvider _services;
        private LogService _dbLogger;
        
        public async Task InitializeAsync(string configPath = DEFAULT_CONFIG_PATH)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile(configPath, false, true)
                .Build();

            _configuration = config;
            _services = BuildServiceProvider();
            _dbLogger = new LogService("Database");

            try
            {
                AbfDbContext.DatabaseUpdated += HandleDatabaseUpdated;
                await using var db = _services.GetRequiredService<AbfDbContext>();
                _dbLogger.Info("Running DB migrations.");
                db.Database.Migrate();
            }
            catch (Exception e)
            {
                _dbLogger.Error("Database migration failed.", e);
                return;
            }

            var ds = _services.GetRequiredService<DiscordService>();

            await ds.SetupAsync(Assembly.GetEntryAssembly());
            await ds.AddExtensionAsync(_services.GetRequiredService<InteractivityExtension>());

            await ds.RunAsync();
            await Task.Delay(Timeout.Infinite);

        }

        private IServiceProvider BuildServiceProvider()
        {
            var collection = new ServiceCollection()
                .Configure<AbfConfiguration>(x => _configuration.GetSection("Bot").Bind(x))
                .AddSingleton<AbfConfigurationProvider>()
                .Configure<DatabaseConfiguration>(x => _configuration.GetSection("Database").Bind(x))
                .AddSingleton<IDatabaseConfigurationProvider, DatabaseConfigurationProvider>()
                .AddSingleton<ConnectionStringProvider>()
                .AddDbContext<AbfDbContext>(ServiceLifetime.Transient)
                .AddSingleton(x => new DiscordBotConfiguration
                {
                    ProviderFactory = _ => x,
                    CommandServiceConfiguration = new CommandServiceConfiguration
                    {
                        IgnoresExtraArguments = true,
                        StringComparison = StringComparison.OrdinalIgnoreCase
                    }
                })
                .AddSingleton<DiscordService>()
                .AddSingleton<InteractivityExtension>();

#if DEBUG

            collection.AddSingleton<IPrefixProvider>(new DefaultPrefixProvider().AddPrefix("!!!").AddMentionPrefix());
#else
            collection.AddSingleton<IPrefixProvider>(new DefaultPrefixProvider().AddPrefix('!').AddMentionPrefix());
#endif

            return collection.BuildServiceProvider();
        }

        private Task HandleDatabaseUpdated(DatabaseActionEventArgs arg)
        {
            if (arg.IsErrored)
                _dbLogger.Error(arg.Path, arg.Exception);
            else
                _dbLogger.Debug(arg.Path);

            return Task.CompletedTask;
        }
    }
}
