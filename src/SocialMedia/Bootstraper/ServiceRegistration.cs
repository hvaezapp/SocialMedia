using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;
using SocialMedia.Infrastructure.Persistence.Context;
using SocialMedia.Services;

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

        public static void RegisterNeo4j(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IDriver>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                return GraphDatabase.Driver(config["GraphDatabase:Url"],
                    AuthTokens.Basic(config["GraphDatabase:Username"], config["GraphDatabase:Password"]));
            });
        }


        public static void RegisterIoc(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<SocialMediaService>();
        }
    }
}
