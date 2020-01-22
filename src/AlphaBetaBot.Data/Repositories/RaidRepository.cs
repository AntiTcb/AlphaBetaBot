using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class RaidRepository : Repository<WowRaid>
    {
        internal RaidRepository(DbSet<WowRaid> entities, AbfDbContext context) : base(entities, context, "Raid") { }
    }
}
