using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class UserRepository : Repository<User>, IGetOrAddRepository<User>
    {
        internal UserRepository(DbSet<User> entities, AbfDbContext context) : base(entities, context, "User") { }

        public async Task<User> GetOrAddAsync(string id)
        {
            _entities.Include(x => x.Characters);

            var entity = await GetAsync(id);
            if (entity is null)
            {
                entity = await AddAsync(new User
                {
                    Id = id
                });
            }

            return entity;
        }
    }
}
