using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    [Table("raids")]
    public class WowRaid : Entity
    {
        public DateTimeOffset RaidTime { get; set; }

        public Raid Raid { get; set; }

        public WowCharacter[] Raiders { get; set; } = new WowCharacter[39];
    }

    public enum Raid
    {
        Onyxia,
        MoltenCore,
        BlackwingLair,
        ZulGurub,
        AQ20,
        AQ40,
        Naxxramas
    }
}
