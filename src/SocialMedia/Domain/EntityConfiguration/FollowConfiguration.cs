using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Entities;

namespace SocialMedia.Domain.EntityConfiguration
{
    public class FollowConfiguration : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.ToTable(Follow.TableName);

            builder.HasKey(a => a.Id);
 
            builder.HasOne(a => a.Follower)
                   .WithMany(u => u.Following)
                   .HasForeignKey(a => a.FollowerId)
                   .OnDelete(DeleteBehavior.Restrict); 


            builder.HasOne(a => a.Following)
                   .WithMany(u => u.Followers)
                   .HasForeignKey(a => a.FollowingId)
                   .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
