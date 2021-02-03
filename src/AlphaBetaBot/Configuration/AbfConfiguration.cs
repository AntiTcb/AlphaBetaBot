using System.Collections.Generic;
using AlphaBetaBot.Data;
using Disqord;

namespace AlphaBetaBot
{
    public class AbfConfiguration
    {
        public string DiscordToken { get; set; }

        public static Dictionary<WowClass, LocalCustomEmoji> ClassEmojis { get; } = new Dictionary<WowClass, LocalCustomEmoji>
        {
            { WowClass.Warrior, new LocalCustomEmoji(671579050489282580, "Warrior") },
            { WowClass.Druid, new LocalCustomEmoji(671579050023845997, "Druid") },
            { WowClass.Rogue,  new LocalCustomEmoji(671579050518773777, "Rogue") },
            { WowClass.Hunter, new LocalCustomEmoji(671579050203938847, "Hunter") },
            { WowClass.Mage, new LocalCustomEmoji(671579050518511648, "Mage") },
            { WowClass.Warlock, new LocalCustomEmoji(671579050505928704, "Warlock") },
            { WowClass.Priest, new LocalCustomEmoji(671579050250338306, "Priest") },
            { WowClass.Paladin, new LocalCustomEmoji(671579050514579486, "Paladin") }
        };

        public static List<IEmoji> RaidEmbedIcons { get; } = new List<IEmoji>(ClassEmojis.Values) { new LocalEmoji("❓"), new LocalEmoji("👋") };
}
}
