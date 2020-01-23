using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class UserRepository : Repository<User>, IGetOrAddRepository<User>
    {
        internal UserRepository(DbSet<User> entities, AbfDbContext context) : base(entities, context, "User") { }

        public override async Task<User> GetAsync(ulong snowflakeId)
        {
            _entities.Include(x => x.Characters);

            var entity = await _entities.FirstOrDefaultAsync(u => u.Id == snowflakeId);
            return entity;
        }

        public async Task<User> GetOrAddAsync(ulong snowflakeId)
        {
            _entities.Include(x => x.Characters);

            var entity = await GetAsync(snowflakeId);
            if (entity is null)
            {
                entity = await AddAsync(new User
                {
                    Id = snowflakeId,
                    Characters = new List<WowCharacter>(),
                    CreatedAt = DateTimeOffset.UtcNow
                });
            }

            return entity;
        }
    }
}
