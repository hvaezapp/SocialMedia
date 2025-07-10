using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SocialMedia.Bootstraper;
using SocialMedia.Domain.Entities;
using SocialMedia.Dtos.Auth;
using SocialMedia.Infrastructure.Persistence.Context;
using SocialMedia.Services;
using SocialMedia.Shared;
using SocialMedia.Shared.Utility;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterMSSSQL();
builder.RegisterIoc();
builder.RegisterNeo4j();
builder.RegisterJWTAuth();

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseAuthorization();
app.UseAuthorization();



app.MapPost("/register", async (RegisterRequestDto registerRequestDto , 
                                SocialMediaDbContext dbContext,
                                SocialMediaService socialMediaService,
                                CancellationToken cancellationToken) =>
{

    // check username is exsist
    var userIsExist = await dbContext.Users.AnyAsync(a => a.Username == registerRequestDto.Username , cancellationToken);
    if (!userIsExist)
        throw new Exception("Current Username is exist try another username.");

    // register user

    // 1- add to sql database
    var newUser = User.Create(registerRequestDto.Fullname, 
                             registerRequestDto.Username ,
                             PasswordHelper.HashPassword(registerRequestDto.Password));

    dbContext.Users.Add(newUser);
    await dbContext.SaveChangesAsync(cancellationToken);

    // 2- add user to neo4j database - create node for this user
    await socialMediaService.CreateUser(newUser.Username);
});


app.MapPost("/login", async (LoginRequestDto loginRequestDto,
                      SocialMediaDbContext dbContext,
                      IConfiguration configuration,
                      CancellationToken cancellationToken) =>
{

    #region check UserExistence
    // check username and password
    var userIsExist = await dbContext.Users
                                     .AnyAsync(a => a.Username == loginRequestDto.Username &&
                                      a.PasswordHash == PasswordHelper.HashPassword(loginRequestDto.Password), 
                                      cancellationToken);
    if (!userIsExist)
        throw new Exception("Username or Password Invalid.");
    #endregion

    #region generate JWT
    // generate jwt token
    var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
    if (jwtSettings == null)
        throw new ArgumentNullException("JwtSettings section not found in configuration.", nameof(jwtSettings));

    var claims = new[]
    {
      new Claim(ClaimTypes.Name, loginRequestDto.Username),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: jwtSettings.Issuer,
        audience: jwtSettings.Audience,
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds
    );

    var jwt = new JwtSecurityTokenHandler().WriteToken(token);
    #endregion

    return Results.Ok(new { token = jwt });

});


app.MapPost("/follow/{currentUsername}/{targetUsername}", async (
    SocialMediaService socialMediaService,
    string currentUsername,
    string targetUsername) =>
{
    await socialMediaService.Follow(currentUsername, targetUsername);

}).RequireAuthorization();



app.MapGet("/suggested-users/{username}", async (SocialMediaService socialMediaService, string username) =>
{
    return Results.Ok((object?)await socialMediaService.SuggestedUsers(username));

}).RequireAuthorization();


app.UseHttpsRedirection();
app.Run();

