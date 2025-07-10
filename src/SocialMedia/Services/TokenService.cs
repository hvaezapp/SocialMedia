using Microsoft.IdentityModel.Tokens;
using SocialMedia.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialMedia.Services;

public class TokenService(IConfiguration configuration)
{
    private readonly IConfiguration _configuration = configuration;


    public string GenerateToken(Claim[] claims)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null)
            throw new ArgumentNullException("JwtSettings section not found in configuration.", nameof(jwtSettings));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
