using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlphaBetaBot.Data
{
    [Table("raid_participants")]
    public class RaidParticipant
    {
        public int Id { get; set; }

        public int CharacterId { get; set; }
        public WowCharacter Character { get; set; }
        public ulong RaidId { get; set; }
        public Raid Raid { get; set; }
        public DateTimeOffset SignedUpAt { get; set; }
        public bool IsTentative { get; set; }
        public bool IsAbsent { get; set; }

        public override string ToString() => $"{Character} | {RaidId} | {Raid.RaidLocationId}";
    }
    public class RaidParticipantConfiguration : IEntityTypeConfiguration<RaidParticipant>
    {
        public void Configure(EntityTypeBuilder<RaidParticipant> builder)
        {
            builder.HasKey(rp => rp.Id);

            builder.Property(rp => rp.Id)
                .ValueGeneratedOnAdd();

            builder.Property(rp => rp.SignedUpAt)
                .ValueGeneratedOnAdd();

            builder.HasOne(rp => rp.Character)
                .WithMany(r => r.RaidsAttending)
                .HasForeignKey(rp => rp.CharacterId);

            builder.HasOne(rp => rp.Raid)
                .WithMany(r => r.Participants)
                .HasForeignKey(rp => rp.RaidId);
        }
    }
}
