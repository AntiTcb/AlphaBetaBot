﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlphaBetaBot.Data
{
    [Table("raids")]
    public class Raid : Entity
    {
        public DateTimeOffset RaidTime { get; set; }

        public RaidLocationId RaidLocationId { get; set; }
        public RaidLocation RaidLocation { get; set; }

        public ICollection<RaidParticipant> Participants { get; set; }

        public override string ToString() => $"{RaidLocationId} | {RaidTime}";
    }

    public class RaidConfiguration : EntityConfiguration<Raid>
    {
        public override void Configure(EntityTypeBuilder<Raid> builder)
        {
            base.Configure(builder);

            builder.HasKey(r => r.Id);
            builder.Property(r => r.Id)
                .ValueGeneratedNever();
            builder.Property(r => r.RaidLocationId)
                .HasConversion<int>();
        }
    }
}
