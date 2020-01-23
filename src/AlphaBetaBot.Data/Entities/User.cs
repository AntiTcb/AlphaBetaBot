using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlphaBetaBot.Data
{
    [Table("users")]
    public class User : Entity
    {
        public ICollection<WowCharacter> Characters { get; set; } = new List<WowCharacter>();
    }

    public class UserConfiguration : EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id)
                .ValueGeneratedNever();
            builder.OwnsMany(u => u.Characters, c =>
            {
                c.WithOwner(c => c.Owner).HasForeignKey("UserId");
            });
        }
    }
}
