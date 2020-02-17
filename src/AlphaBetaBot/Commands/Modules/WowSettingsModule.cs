using System.Threading.Tasks;
using Disqord;
using Qmmands;

namespace AlphaBetaBot
{
    [Name("Settings"), Group("settings")]
    [RequireGuild]
    public class WowSettingsModule : AbfModuleBase
    {
        [Command("signupchannel")]
        public async Task SignupChannelAsync(CachedTextChannel channel = null)
        {
            ulong? channelId = DbContext.Guild.RaidSignupChannelId;

            if (channel is null && channelId is null)
            {
                await ReplyAsync("No raid signup channel exists.");
            }
            else if (channel is null)
            {
                channel = Context.Guild.GetChannel(channelId.Value) as CachedTextChannel;
                await ReplyAsync($"The raid sigup channel is {channel}");
            }
            else
            {
                DbContext.Guild.RaidSignupChannelId = channel.Id;
                await ReplyAsync($"The raid signup channel is now set to {channel}.");
            }
        }
    }
}
