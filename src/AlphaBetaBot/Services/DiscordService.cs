using Disqord;
using Disqord.Bot;
using Disqord.Bot.Prefixes;

namespace AlphaBetaBot
{
    public class DiscordService : DiscordBot
    {
        private readonly LogService _logger;
        private readonly AbfConfiguration _configuration;

        public DiscordService(AbfConfigurationProvider configProvider, IPrefixProvider prefixProvider, DiscordBotConfiguration discordConfig = null) 
            : base(TokenType.Bot, configProvider.Configuration.DiscordToken, prefixProvider, discordConfig)
        {
            _logger = LogService.GetLogger("Discord");
            _configuration = configProvider.Configuration;
        }
    }
}
