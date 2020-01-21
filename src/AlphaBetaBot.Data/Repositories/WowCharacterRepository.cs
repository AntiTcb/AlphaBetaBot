using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class WowCharacterRepository : Repository<WowCharacter>
    {
        internal WowCharacterRepository(DbSet<WowCharacter> entities, AbfDbContext context) : base(entities, context, "WowCharacter") { }
    }
}
