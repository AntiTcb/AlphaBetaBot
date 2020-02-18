using System.Linq;
using System.Threading.Tasks;
using AlphaBetaBot.Data;

namespace AlphaBetaBot
{
    public sealed class DatabaseCommandContext
    {
        private readonly AbfCommandContext _ctx;

        private readonly IGetOrAddRepository<User> _users;
        //private readonly IRepository<WowCharacter> _characters;
        private readonly IRepository<Raid> _raids;
        private readonly IGetOrAddRepository<Guild> _guilds;

        public bool IsReady { get; private set; }

        public User User { get; private set; }
        public Guild Guild { get; private set; }
        
        public AbfDbContext Database { get; }

        public DatabaseCommandContext(AbfCommandContext ctx, AbfDbContext context)
        {
            _ctx = ctx;
            _users = context.RequestRepository<IGetOrAddRepository<User>>();
            _raids = context.RequestRepository<IRepository<Raid>>();
            
            if (!(_ctx.Guild is null))
                _guilds = context.RequestRepository<IGetOrAddRepository<Guild>>();

            Database = context;
        }

        public async Task PrepareAsync()
        {
            User = await (_users as UserRepository).GetOrAddAsync(_ctx.User.Id);

            if (!(_ctx.Guild is null))
                Guild = await (_guilds as GuildRepository).GetOrAddAsync(_ctx.Guild.Id);

            IsReady = true;
        }

        public Task AddRaidAsync(Raid raid) => _raids.AddAsync(raid);

        public async Task<Raid> GetRaidAsync(ulong raidSignupMessageId) 
            => await _raids.GetAsync(raidSignupMessageId);

        public async Task SignupToRaidAsync(RaidParticipant raidParticipant)
        {
            var raid = await _raids.GetAsync(raidParticipant.RaidId);
        }
        public Task UpdateUserAsync() => _users.UpdateAsync(User);
    }
}
