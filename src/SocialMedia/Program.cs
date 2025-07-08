using Neo4j.Driver;
using Scalar.AspNetCore;
using SocialMedia.Bootstraper;
using SocialMedia.Dtos;
using SocialMedia.Services;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterMSSSQL();
builder.RegisterIoc();
builder.RegisterNeo4j();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapPost("/create-user/{username}", async (SocialMediaService socialMediaService, string username) =>
{
    await socialMediaService.CreateUser(username);
});


app.MapPost("/follow/{currentUsername}/{targetUsername}", async (
    SocialMediaService socialMediaService,
    string currentUsername,
    string targetUsername) =>
{
    await socialMediaService.Follow(currentUsername, targetUsername);

});



app.MapGet("/suggested-users/{username}", async (SocialMediaService socialMediaService, string username) =>
    Results.Ok((object?)await socialMediaService.SuggestedUsers(username)));




app.UseHttpsRedirection();
app.Run();

