using System.Threading.Tasks;
using AlphaBetaBot.Data;

namespace AlphaBetaBot
{
    public sealed class DatabaseCommandContext
    {
        private readonly AbfCommandContext _ctx;

        private readonly IGetOrAddRepository<User> _users;
        private readonly IRepository<WowCharacter> _characters;
        private readonly IGetOrAddRepository<WowRaid> _raids;

        public bool IsReady { get; private set; }

        public User User { get; private set; }
        
        public AbfDbContext Database { get; }

        public DatabaseCommandContext(AbfCommandContext ctx, AbfDbContext context)
        {
            _ctx = ctx;
            _users = context.RequestRepository<IGetOrAddRepository<User>>();
            _characters = context.RequestRepository<IRepository<WowCharacter>>();
            _raids = context.RequestRepository<IGetOrAddRepository<WowRaid>>();
            Database = context;
        }

        public async Task PrepareAsync()
        {
            User = await (_users as UserRepository).GetOrAddAsync(_ctx.User.Id.ToString());

            IsReady = true;
        }

        public Task UpdateUserAsync() => _users.UpdateAsync(User);
    }
}
