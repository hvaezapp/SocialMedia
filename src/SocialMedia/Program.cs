using Microsoft.EntityFrameworkCore;
using SocialMedia.Infrastructure.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SocialMediaDbContext>(configure =>
{
    configure.UseSqlServer(builder.Configuration.GetConnectionString(SocialMediaDbContext.DefaultConnectionStringName));
});

var app = builder.Build();

app.Run();

