using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.Entities;
using SocialMedia.Dtos.Auth;
using SocialMedia.Infrastructure.Persistence.Context;
using SocialMedia.Shared.Utility;
using System.Security.Claims;

namespace SocialMedia.Services;

public class AuthService(SocialMediaDbContext dbContext, GraphService socialMediaService, TokenService tokenService)
{
    private readonly SocialMediaDbContext _dbContext = dbContext;
    private readonly GraphService _socialMediaService = socialMediaService;
    private readonly TokenService _tokenService = tokenService;

    internal async Task Register(RegisterRequestDto registerRequestDto, CancellationToken cancellationToken)
    {
        // check username is exsist
        var UserameIsExist = await _dbContext.Users.AnyAsync(a => a.Username == registerRequestDto.Username, cancellationToken);
        if (UserameIsExist)
            throw new Exception("Username is exist try another username.");

        // register user

        // 1- add to sql database
        var newUser = User.Create(registerRequestDto.Fullname,
                                 registerRequestDto.Username,
                                 PasswordHelper.HashPassword(registerRequestDto.Password));

        _dbContext.Users.Add(newUser);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 2- add user to neo4j database - create node for this user
        await _socialMediaService.CreateUser(newUser.Id);
    }

    internal async Task<LoginResponceDto> Login(LoginRequestDto loginRequestDto, CancellationToken cancellationToken)
    {

        // check username and password
        var user = await _dbContext.Users.FirstOrDefaultAsync(a => a.Username == loginRequestDto.Username &&
                                          a.PasswordHash == PasswordHelper.HashPassword(loginRequestDto.Password),
                                          cancellationToken);

        if (user is null)
            throw new Exception("Username or Password Invalid.");


        // set claims and generate jwt token
        var claims = new[]
        {
          new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
          new Claim(ClaimTypes.Name, user.Username),
        };


        var jwtToken = _tokenService.GenerateToken(claims);
        return new LoginResponceDto(jwtToken);
    }
}
