using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class UserRepository : Repository<User>, IGetOrAddRepository<User>
    {
        internal UserRepository(DbSet<User> entities, AbfDbContext context) : base(entities, context, "User") { }

        public async Task<User> GetAsync(string snowflakeId)
        {
            _entities.Include(x => x.Characters);

            var entity = await _entities.FirstOrDefaultAsync(u => u.SnowflakeId == snowflakeId);
            return entity;
        }

        public async Task<User> GetOrAddAsync(string snowflakeId)
        {
            _entities.Include(x => x.Characters);

            var entity = await GetAsync(snowflakeId);
            if (entity is null)
            {
                entity = await AddAsync(new User
                {
                    SnowflakeId = snowflakeId,
                    Characters = new List<WowCharacter>()
                });
            }

            return entity;
        }

        public Task<User> GetOrAddAsync(int id) => throw new System.NotImplementedException();
    }
}
