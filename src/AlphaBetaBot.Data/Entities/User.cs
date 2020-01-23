using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlphaBetaBot.Data
{
    [Table("users")]
    public class User : DiscordEntity
    {
        public ICollection<WowCharacter> Characters { get; set; } 
    }
}
