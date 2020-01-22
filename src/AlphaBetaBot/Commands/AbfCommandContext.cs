using System;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Bot;
using Disqord.Bot.Prefixes;
using Microsoft.Extensions.DependencyInjection;

namespace AlphaBetaBot
{
    public sealed class AbfCommandContext : DiscordCommandContext, IAsyncDisposable
    {
        public CachedUser Self => Bot.CurrentUser;
        public CachedMember SelfMember => Guild.CurrentMember;

        private readonly DatabaseCommandContext _databaseContext;

        public AbfCommandContext(DiscordBot bot, CachedUserMessage message, IPrefix prefix)
            : base(bot, prefix, message) 
            => _databaseContext = new DatabaseCommandContext(this, ServiceProvider.GetRequiredService<AbfDbContext>());

        public DatabaseCommandContext DatabaseContext
        {
            get
            {
                if (!_databaseContext.IsReady)
                {
                    throw new InvalidOperationException(
                        "The database context is not ready. Please make sure it's prepared.");
                }

                return _databaseContext;
            }
        }

        public Task PrepareAsync() => _databaseContext.PrepareAsync();

        public Task EndAsync() => _databaseContext.Database.SaveChangesAsync();

        public ValueTask DisposeAsync() => _databaseContext.Database.DisposeAsync();
    }
}
