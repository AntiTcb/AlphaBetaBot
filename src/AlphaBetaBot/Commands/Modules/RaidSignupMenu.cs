using System.Threading.Tasks;
using AlphaBetaBot.Data;
using Disqord;
using Disqord.Extensions.Interactivity.Menus;

namespace AlphaBetaBot
{
    public class RaidSignupMenu : MenuBase
    {
        private Raid _raid;
        private DatabaseCommandContext _dbContext;

        public RaidSignupMenu(Snowflake messageId, DatabaseCommandContext dbContext) : base()
        {

        }
        
        public RaidSignupMenu(Raid raid, DatabaseCommandContext dbContext) : base()
        {
            _raid = raid;
            _dbContext = dbContext;
        }
        protected override async Task<IUserMessage> InitialiseAsync()
        {
            var message = await Channel.SendMessageAsync($"Signups for the {_raid.RaidLocationId} raid at {_raid.RaidTime} have started! Please use the below buttons to sign up your character to the raid.");
            return message;
        }

        [Button("")]
        public async Task SignupWarriorAsync(ButtonEventArgs e)
        {
            if (e.WasAdded)
            {
                var user = await e.User.Downloadable.GetOrDownloadAsync();
            }
            else
            {

            }
        }
    }
}
