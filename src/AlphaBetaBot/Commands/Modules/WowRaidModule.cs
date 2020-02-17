using System.Linq;
using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Rest;
using Humanizer;
using Qmmands;

namespace AlphaBetaBot
{
    [Name("raid"), Group("raid", "raids")]
    [RequireOwner(Group = "perm")]
    [RequireUserPermissions(Permission.ManageGuild, Group = "perm")]
    [RequireGuild]
    public class WowRaidModule : AbfModuleBase
    {
        [Command("add", "create")]
        public async Task AddRaidAsync(RaidLocationId raidLocation, [Remainder] string raidTime)
        {
            ulong channelId = DbContext.Guild.RaidSignupChannelId ?? Context.Channel.Id;

            var signupChannel = Context.Guild.GetTextChannel(channelId);

            var signupMessage = await signupChannel.SendMessageAsync($"Signups for {raidLocation.Humanize()} at {raidTime} have started. Click the reaction with your class icon to sign up!");

            var tasks = AbfConfiguration.ClassEmojis.Values.Select(async ce => await signupMessage.AddReactionAsync(ce));

            await Task.WhenAll(tasks);

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
            var embed = new LocalEmbedBuilder().WithTitle($"{raid.RaidLocationId.Humanize()} - {raid.RaidTime}");

            var lines = raid.Participants.OrderBy(p => p.SignedUpAt).Select(p => $"{AbfConfiguration.ClassEmojis[p.Character.Class]} | {p.Character.CharacterName} | {p.Character.Role.Humanize()[0]}");
            var roleCounts = raid.Participants.GroupBy(p => p.Character.Role).Select(g => (Role: g.Key, Count: g.Count()));
            var classCounts = raid.Participants.GroupBy(rp => rp.Character.Class).Select(g => (Class: g.Key, Count: g.Count()));

            embed.WithDescription(string.Join('\n', lines));

            //foreach (var (Role, Count) in roleCounts)
            //    embed.AddField(Role.ToString(), Count);

            foreach (var (Class, Count) in classCounts)
                embed.AddField(Class.ToString(), Count, true);

            embed.AddField("Total", raid.Participants.Count(), true);

            embed.WithFooter(string.Join("roleCounts.Select(r => $"{}");

            await msg.ModifyAsync(m => m.Embed = embed.Build());
        }
    }
}
