using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Entities;
using System.Reflection;

namespace SocialMedia.Infrastructure.Persistence.Context
{
    public class SocialMediaDbContext(DbContextOptions dbContextOptions) : DbContext(dbContextOptions)
    {

        private const string DefaultSchema = "socialmedia";
        public const string DefaultConnectionStringName = "SvcDbContext";

        public DbSet<User> Users => Set<User>();
        public DbSet<Follow> Follows => Set<Follow>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(DefaultSchema);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

    }
}
