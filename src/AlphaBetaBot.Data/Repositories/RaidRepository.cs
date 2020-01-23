using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class RaidRepository : Repository<Raid>
    {
        internal RaidRepository(DbSet<Raid> entities, AbfDbContext context) : base(entities, context, "Raid") { }
    }
}
