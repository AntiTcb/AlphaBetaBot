using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class GuildRepository : Repository<Guild>, IGetOrAddRepository<Guild>
    {
        internal GuildRepository(DbSet<Guild> entities, AbfDbContext context) : base(entities, context, "Guild") { }

        public async Task<Guild> GetOrAddAsync(ulong snowflakeId)
        {
            var entity = await GetAsync(snowflakeId);
            if (entity is null)
            {
                entity = await AddAsync(new Guild
                {
                    Id = snowflakeId
                });
            }

            return entity;
        }
    }
}
