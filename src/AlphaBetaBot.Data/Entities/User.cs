using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    [Table("users")]
    public class User : DiscordEntity
    {
        public List<WowCharacter> Characters { get; set; } = new List<WowCharacter>();
    }
}
