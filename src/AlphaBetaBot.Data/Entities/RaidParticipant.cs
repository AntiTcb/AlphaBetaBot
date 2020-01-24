using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlphaBetaBot.Data
{
    [Table("raid_participants")]
    public class RaidParticipant : Entity
    {
        public int CharacterId { get; set; }
        public WowCharacter Character { get; set; }
        public ulong RaidId { get; set; }
        public Raid Raid { get; set; }
    }
    public class RaidParticipantConfiguration : EntityConfiguration<RaidParticipant>
    {
        public override void Configure(EntityTypeBuilder<RaidParticipant> builder)
        {
            base.Configure(builder);

            builder.HasKey(rp => new { rp.RaidId, rp.CharacterId });

            builder.HasOne(rp => rp.Character)
                .WithMany(r => r.RaidsAttending)
                .HasForeignKey(rp => rp.CharacterId);

            builder.HasOne(rp => rp.Raid)
                .WithMany(r => r.Participants)
                .HasForeignKey(rp => rp.RaidId);
        }
    }
}
