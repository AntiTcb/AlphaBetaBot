using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    [Table("raids")]
    public class Raid : Entity
    {
        public DateTimeOffset RaidTime { get; set; }

        public RaidLocationId RaidLocationId { get; set; }
        public RaidLocation RaidLocation { get; set; }

        public ICollection<RaidParticipant> Participants { get; set; }
    }

    public class RaidParticipant
    {
        public int CharacterId { get; set; }
        public WowCharacter Character { get; set; }
        public int RaidId { get; set; }
        public Raid Raid { get; set; }
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
