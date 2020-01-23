using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlphaBetaBot.Data
{
    [Table("raid_locations")]
    public class RaidLocation
    {
        [Key]
        public RaidLocationId RaidLocationId { get; set; }
        public string Name { get; set; }
    }
    public class RaidLocationConfiguration : IEntityTypeConfiguration<RaidLocation>
    {
        public void Configure(EntityTypeBuilder<RaidLocation> builder)
        {
            builder.Property(r => r.RaidLocationId)
                .HasConversion<int>();

            builder.HasData(Enum.GetValues(typeof(RaidLocationId))
                .Cast<RaidLocationId>()
                .Select(e => new RaidLocation
                {
                    Name = e.ToString(),
                    RaidLocationId = e
                }));
        }
    }
}
