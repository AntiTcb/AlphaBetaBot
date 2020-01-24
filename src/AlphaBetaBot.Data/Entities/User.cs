using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlphaBetaBot.Data
{
    [Table("users")]
    public class User : Entity
    {
        public User() => Characters = new List<WowCharacter>();

        public virtual ICollection<WowCharacter> Characters { get; set; }
    }

    public class UserConfiguration : EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedNever();
            builder.HasMany(u => u.Characters)
                .WithOne(c => c.Owner);
        }
    }
}
