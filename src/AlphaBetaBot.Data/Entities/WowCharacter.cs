using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    [Table("characters")]
    public class WowCharacter : Entity
    {
        public User Owner { get; set; }

        [Required]
        public string CharacterName { get; set; }

        public WowClass Class { get; set; }

        public ClassRole Role { get; set; }
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
