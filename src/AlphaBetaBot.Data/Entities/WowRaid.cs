using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    [Table("raids")]
    public class WowRaid : Entity
    {
        public DateTimeOffset RaidTime { get; set; }

        public RaidLocationId RaidLocationId { get; set; }
        public RaidLocation Raid { get; set; }

        public List<WowCharacter> Raiders { get; set; } = new List<WowCharacter>();
    }

    [Table("raid_locations")]
    public class RaidLocation
    {
        [Key]
        public RaidLocationId RaidLocationId { get; set; }
        public string Name { get; set; }

    }

    public enum RaidLocationId : int
    {
        Onyxia = 0,
        MoltenCore = 1,
        BlackwingLair = 2,
        ZulGurub = 3,
        AQ20 = 4,
        AQ40 = 5,
        Naxxramas = 6
    }
}
