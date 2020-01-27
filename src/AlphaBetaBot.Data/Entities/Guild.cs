using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlphaBetaBot.Data
{
    [Table("guilds")]
    public class Guild : Entity
    {
        public ulong? RaidSignupChannelId { get; set; } 
    }

    public class GuildConfiguration : EntityConfiguration<Guild>
    {
        public override void Configure(EntityTypeBuilder<Guild> builder)
        {
            base.Configure(builder);

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id)
                .ValueGeneratedNever();
        }
    }
}
