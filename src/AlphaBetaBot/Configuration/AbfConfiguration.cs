using Disqord;

namespace AlphaBetaBot
{
    public class AbfConfiguration
    {
        public string DiscordToken { get; set; }

        public static (string Name, ulong Id)[] ClassEmotes { get; } = new[] {
            ("Warrior", 671579050489282580UL),
            ("Druid", 671579050023845997UL),
            ("Rogue", 671579050518773777UL),
            ("Hunter", 671579050203938847UL),
            ("Mage", 671579050518511648UL),
            ("Warlock", 671579050505928704UL),
            ("Priest", 671579050250338306UL),
            ("Paladin", 671579050514579486UL)
        };

        public static LocalCustomEmoji[] ClassEmojis { get; } = new[]
        {
            new LocalCustomEmoji(671579050489282580, "Warrior"),
            new LocalCustomEmoji(671579050023845997, "Druid"),
            new LocalCustomEmoji(671579050518773777, "Rogue"),
            new LocalCustomEmoji(671579050203938847, "Hunter"),
            new LocalCustomEmoji(671579050518511648, "Mage"),
            new LocalCustomEmoji(671579050505928704, "Warlock"),
            new LocalCustomEmoji(671579050250338306, "Priest"),
            new LocalCustomEmoji(671579050514579486, "Paladin")
        };
    }
}
