using Scalar.AspNetCore;
using SocialMedia.Bootstraper;
using SocialMedia.Dtos.Auth;
using SocialMedia.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterMSSSQL();
builder.RegisterIoc();
builder.RegisterNeo4j();
builder.RegisterJWTAuth();
builder.RegisterCommon();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthorization();
app.UseAuthorization();


// register user
app.MapPost("/register", async (RegisterRequestDto registerRequestDto,
                                AuthService authService,
                                CancellationToken cancellationToken) =>
{
    await authService.Register(registerRequestDto, cancellationToken);

});


// login user
app.MapPost("/login", async (LoginRequestDto loginRequestDto,
                             AuthService authService,
                             CancellationToken cancellationToken) =>
{

    var result = await authService.Login(loginRequestDto, cancellationToken);
    return Results.Ok(result);

});



// follow target user by id
app.MapPost("/follow/{targetUserid}", async (SocialMediaService socialMediaService, 
                                             HttpContext context, 
                                             long targetUserid,
                                             CancellationToken cancellationToken) =>
{
    var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    await socialMediaService.Follow(long.Parse(currentUserId), targetUserid , cancellationToken);

}).RequireAuthorization();



// Unfollow target user by id
app.MapPost("/Unfollow/{targetUserid}", async (SocialMediaService socialMediaService,
                                              HttpContext context,
                                              long targetUserid,
                                              CancellationToken cancellationToken) =>
{
    var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    await socialMediaService.Unfollow(long.Parse(currentUserId), targetUserid, cancellationToken);

}).RequireAuthorization();




// suggest user to loged in user to follow
app.MapGet("/suggested-users", async (SocialMediaService socialMediaService,
                                      HttpContext context,
                                      CancellationToken cancellationToken) =>
{
    var currentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
    return Results.Ok(await socialMediaService.SuggestedUsers(long.Parse(currentUserId) , cancellationToken));

}).RequireAuthorization();




app.UseHttpsRedirection();
app.Run();

