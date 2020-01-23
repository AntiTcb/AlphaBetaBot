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

        public ICollection<RaidParticipant> RaidsAttending { get; set; }
    }

    public enum WowClass
    {
        Druid,
        Hunter,
        Mage,
        Paladin,
        Priest,
        Rogue,
        Shaman,
        Warlock,
        Warrior
    }

    public enum ClassRole
    {
        CasterDps,
        Healer,
        MeleeDps,
        Tank
    }
}
