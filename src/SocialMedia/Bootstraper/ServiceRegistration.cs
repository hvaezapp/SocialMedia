using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Neo4j.Driver;
using SocialMedia.Infrastructure.Persistence.Context;
using SocialMedia.Services;
using System.Text;

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


        public static void RegisterJWTAuth(this WebApplicationBuilder builder)
        {
            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null)
                throw new ArgumentNullException("JwtSettings section not found in configuration." , nameof(jwtSettings));

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });
        }
    }
}
