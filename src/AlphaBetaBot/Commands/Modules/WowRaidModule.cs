using System;
using System.Linq;
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

            var signupMessage = await signupChannel.SendMessageAsync($"@everyone Signups for {raidLocation.Humanize().Transform(To.TitleCase)} at {raidTime} have started. Click the reaction with your class icon to sign up!");

            var tasks = AbfConfiguration.ClassEmojis.Values.Select(async ce => await signupMessage.AddReactionAsync(ce));
            await Task.WhenAll(tasks);

            await signupMessage.AddReactionAsync(new LocalEmoji("❓"));

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

        public static async Task CreateRaidEmbedAsync(RestUserMessage msg, Raid raid)
        {
            var westernTimeZone = TZConvert.GetTimeZoneInfo("America/Los_Angeles");
            var localDateTime = TimeZoneInfo.ConvertTime(raid.RaidTime, westernTimeZone);

            if (DateTimeOffset.UtcNow.CompareTo(localDateTime) > 0)
            {
                await msg.ModifyAsync(m => m.Content = "This raid has already occurred.");
                await msg.ClearReactionsAsync();
                return;
            }

            if (!msg.IsPinned)
                await msg.PinAsync();

            var embed = new LocalEmbedBuilder()
                .WithTitle($"{raid.RaidLocationId.Humanize().Transform(To.TitleCase)} - {localDateTime:MM/dd @ hh tt}");

            var lines = raid.Participants.OrderBy(p => p.SignedUpAt).Select(p => $"{AbfConfiguration.ClassEmojis[p.Character.Class]} | {p.Character.CharacterName} | {p.Character.Role.Humanize()[0]}{(p.IsTentative ? " -- Tentative" : "")}");
            var roleCounts = Enum.GetNames(typeof(ClassRole)).Select(r => (Role: Enum.Parse<ClassRole>(r), Count: raid.Participants.Count(rp => rp.Character.Role == Enum.Parse<ClassRole>(r))));
            var classCounts = Enum.GetNames(typeof(WowClass)).Select(c => (Class: Enum.Parse<WowClass>(c), Count: raid.Participants.Count(rp => rp.Character.Class == Enum.Parse<WowClass>(c))));

            embed.WithDescription(string.Join('\n', lines));

            foreach (var (Class, Count) in classCounts)
                embed.AddField(Class.ToString(), Count, true);

            embed.AddField("Total", raid.Participants.Count(), true);

            embed.WithFooter(string.Join(" - ", roleCounts.Select(rc => $"{rc.Role.Humanize()}: {rc.Count}")));

            embed.WithTimestamp(raid.RaidTime);

            await msg.ModifyAsync(m => m.Embed = embed.Build());
        }
    }
}
