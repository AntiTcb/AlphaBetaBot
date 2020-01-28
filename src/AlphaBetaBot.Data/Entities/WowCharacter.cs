using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    [Table("characters")]
    public class WowCharacter 
    {
        public int Id { get; set; }
        public User Owner { get; set; }

        [Required]
        public string CharacterName { get; set; }

        [Required]
        public WowClass Class { get; set; }

        [Required]
        public ClassRole Role { get; set; }

        public virtual ICollection<RaidParticipant> RaidsAttending { get; set; }

        public override string ToString() => $"{CharacterName} | {Class} | {Role}"; 
    }

    public enum WowClass
    {
        Druid,
        Hunter,
        Mage,
        Paladin,
        Priest,
        Rogue,
        Warlock,
        Warrior
    }

    public enum ClassRole
    {
        Ranged,
        Healer,
        Melee,
        Tank
    }
}
