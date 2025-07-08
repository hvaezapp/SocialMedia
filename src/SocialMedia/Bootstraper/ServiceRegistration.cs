using Microsoft.EntityFrameworkCore;
using SocialMedia.Infrastructure.Persistence.Context;

namespace SocialMedia.Bootstraper
{
    public static class ServiceRegistration
    {
        public static void RegisterMSSSQL(this WebApplicationBuilder builder)
        {

            builder.Services.AddDbContext<SocialMediaDbContext>(configure =>
            {
                configure.UseSqlServer(builder.Configuration.GetConnectionString(SocialMediaDbContext.DefaultConnectionStringName));
            });
        }
    }
}
