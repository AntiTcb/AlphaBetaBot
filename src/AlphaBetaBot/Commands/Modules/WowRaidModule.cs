using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Rest;
using Humanizer;
using Qmmands;
using TimeZoneConverter;

namespace AlphaBetaBot
{
    [Name("Raids"), Group("raid", "raids")]
    public class WowRaidModule : AbfModuleBase
    {
        [Command("add", "create")]
        [Description("Creates a raid for everyone to sign up for!")]
        [Remarks("!raid create BWL 03/10 7PM")]
        public async Task AddRaidAsync([OverrideTypeParser(typeof(RaidLocationParser))] [Description("Raid dungeon: MC|Ony|BWL|ZG|AQ20/40|Naxx")] RaidLocationId raidLocation, [Description("Raid time: MM/DD HH AM/PM format!")] [Remainder] RaidTime raidTime)
        {
            ulong channelId = DbContext.Guild.RaidSignupChannelId ?? Context.Channel.Id;

            var signupChannel = Context.Guild.GetTextChannel(channelId);

            var signupMessage = await signupChannel.SendMessageAsync($"@everyone Signups for {raidLocation.Humanize().Transform(To.TitleCase)} at {raidTime} have started. If you have your character registered with the bot, click the reaction with your class icon to sign up!");
            await signupMessage.PinAsync();
            await AddRaidEmojisAsync(signupMessage);

            var raid = new Raid
            {
                Id = signupMessage.Id,
                RaidLocationId = raidLocation,
                RaidTime = raidTime
            };

            await DbContext.AddRaidAsync(raid);

            if (signupChannel.Id != Context.Channel.Id)
                await ReplyAsync($"Raid signups started!");
        }

        private static async Task AddRaidEmojisAsync(RestUserMessage signupMessage)
        {

            foreach (var icon in AbfConfiguration.RaidEmbedIcons)
            {
                if (!signupMessage.Reactions.ContainsKey(icon))
                    await signupMessage.AddReactionAsync(icon);
            }
        }

        public static async Task CreateRaidEmbedAsync(RestUserMessage msg, Raid raid)
        {
            var westernTimeZone = TZConvert.GetTimeZoneInfo("America/Los_Angeles");
            var localDateTime = TimeZoneInfo.ConvertTime(raid.RaidTime, westernTimeZone);

            if (DateTimeOffset.UtcNow.CompareTo(raid.RaidTime) > 0)
            {
                if (msg.IsPinned)
                    await msg.UnpinAsync();
                await msg.ModifyAsync(m => m.Content = "This raid has already occurred.");
                await msg.ClearReactionsAsync();
                return;
            }

            await AddRaidEmojisAsync(msg);

            if (!msg.IsPinned)
                await msg.PinAsync();

            var roleCounts = Enum.GetNames(typeof(ClassRole)).Select(r => (Role: Enum.Parse<ClassRole>(r), Count: raid.Participants.Where(rp => !rp.IsAbsent).Count(rp => rp.Character.Role == Enum.Parse<ClassRole>(r))));
            var classCounts = Enum.GetNames(typeof(WowClass)).Select(c => (Class: Enum.Parse<WowClass>(c), Count: raid.Participants.Where(rp => !rp.IsAbsent).Count(rp => rp.Character.Class == Enum.Parse<WowClass>(c))));

            var embed = new LocalEmbedBuilder()
                .WithTitle($"{raid.RaidLocationId.Humanize().Transform(To.TitleCase)} - {localDateTime:MM/dd @ hh tt}")
                .WithTimestamp(raid.RaidTime);

            var raiderGroups = raid.Participants.Where(rp => !rp.IsAbsent).ToArray().OrderBy(rp => rp.SignedUpAt)
                .Select((rp, index) => (SignedUpNumber: index + 1, RaidParticipant: rp))
                .GroupBy(signup => signup.RaidParticipant.Character.Class)
                .Select(g => (Class: Enum.Parse<WowClass>(g.Key.ToString()), Raiders: g.OrderBy(r => r.SignedUpNumber)))
                .ToDictionary(k => k.Class, v => v.Raiders);

            var embedLayoutDict = new Dictionary<WowClass, IOrderedEnumerable<(int, RaidParticipant)>> {
                { WowClass.Druid, null },
                { WowClass.Hunter, null },
                { WowClass.Mage, null },
                { WowClass.Paladin, null },
                { WowClass.Priest, null },
                { WowClass.Rogue, null },
                { WowClass.Warlock, null },
                { WowClass.Warrior, null }
            };

            foreach (var (Class, _) in embedLayoutDict)
            {
                bool hasRaiders = raiderGroups.TryGetValue(Class, out var Raiders);

                var field = new LocalEmbedFieldBuilder
                {
                    Name = $"{AbfConfiguration.ClassEmojis[Class]} {Class.Humanize().Pluralize()} ({Raiders?.Count() ?? 0})",
                    IsInline = true
                }.WithBlankValue();

                if (!hasRaiders)
                {
                    embed.AddField(field);
                    continue;
                }

                var sb = new StringBuilder();
                foreach (var (signupNumber, raider) in Raiders)
                    sb.AppendLine($"{signupNumber}) {raider.Character.CharacterName} | {raider.Character.Role.Humanize()[0]}{(raider.IsTentative ? $" | {new LocalEmoji("❓")}" : "")}");

                field.Value = sb.ToString();
                embed.AddField(field);
            }

            embed.AddField($"Total ({raid.Participants.Where(rp => !rp.IsAbsent).Count()})", string.Join("\n", new[] { string.Join("\n", roleCounts.Select(rc => $"{rc.Role.Humanize()} ({rc.Count})")), $"Tentative: {raid.Participants.Count(rp => rp.IsTentative)}"}), true);

            if (raid.Participants.Any(rp => rp.IsAbsent))
                embed.AddField($"Absent ({raid.Participants.Count(rp => rp.IsAbsent)})", string.Join("\n", raid.Participants.Where(rp => rp.IsAbsent).Select(rp => rp.Character.CharacterName)));

            await msg.ModifyAsync(m => m.Embed = embed.Build());
        }
    }
}
