using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    public abstract class Entity
    {
        [Column("key"), Key]
        public int Id { get; set; }

        [Column("created_at"), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        [Column("last_updated_at"), DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset LastUpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    }

    public abstract class DiscordEntity : Entity
    {
        [Column("snowflake_id")]
        public string SnowflakeId { get; set; }
    }
}
