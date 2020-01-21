using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class AbfDbContext : DbContext
    {
        private readonly IReadOnlyList<object> _repositories;
        private readonly ConnectionStringProvider _connectionStringProvider;

        public DbSet<User> Users { get; set; }
        public DbSet<WowCharacter> Characters { get; set; }
        public DbSet<WowRaid> Raids { get; set; }

        public static Func<DatabaseActionEventArgs, Task> DatabaseUpdated;

        public AbfDbContext(ConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;

            var userRepo = new UserRepository(Users, this);
            var characterRepo = new WowCharacterRepository(Characters, this);
            var raidRepo = new RaidRepository(Raids, this);

            var repos = new List<object> { userRepo, characterRepo, raidRepo };

            _repositories = repos.AsReadOnly();
        }

        internal void InvokeEvent(DatabaseActionEventArgs e)
        {
            DatabaseUpdated?.Invoke(e);
        }
    }
}
