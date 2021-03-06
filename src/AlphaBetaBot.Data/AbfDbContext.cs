﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AlphaBetaBot.Data
{
    public class AbfDbContext : DbContext
    {
        private readonly IReadOnlyList<object> _repositories;
        private readonly ConnectionStringProvider _connectionStringProvider;

        public DbSet<User> Users { get; set; }
        //public DbSet<WowCharacter> Characters { get; set; }
        public DbSet<Guild> Guilds { get; set; }
        public DbSet<Raid> Raids { get; set; }

        public static Func<DatabaseActionEventArgs, Task> DatabaseUpdated;

        public AbfDbContext(ConnectionStringProvider connectionStringProvider)
        {
            _connectionStringProvider = connectionStringProvider;

            var guildRepo = new GuildRepository(Guilds, this);
            var userRepo = new UserRepository(Users, this);
            //var characterRepo = new WowCharacterRepository(Characters, this);
            var raidRepo = new RaidRepository(Raids, this);

            var repos = new List<object> { userRepo, raidRepo, guildRepo };

            _repositories = repos.AsReadOnly();
        }
        public T RequestRepository<T>() => _repositories.OfType<T>().FirstOrDefault();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(_connectionStringProvider.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UserConfiguration());
            builder.ApplyConfiguration(new RaidConfiguration());
            builder.ApplyConfiguration(new RaidLocationConfiguration());
            builder.ApplyConfiguration(new RaidParticipantConfiguration());
            builder.ApplyConfiguration(new GuildConfiguration());
        }

        internal void InvokeEvent(DatabaseActionEventArgs e) => DatabaseUpdated?.Invoke(e);
    }
}
