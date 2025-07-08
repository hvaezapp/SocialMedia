using Scalar.AspNetCore;
using SocialMedia.Bootstraper;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterMSSSQL();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapGet("/" , () => { });

app.Run();

