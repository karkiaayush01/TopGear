using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Auth;

namespace TopGear.Application.Services;

public class JwtTokenService(IOptions<JwtOptions> jwtOptions, UserManager<User> userManager) : IJwtTokenService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<String> GenerateUserToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!)
        };

        // Add roles in claim
        var roles = await userManager.GetRolesAsync(user);
        string role = roles.FirstOrDefault() ?? "Customer";
        claims.Add(new Claim(ClaimTypes.Role, role));

        return GenerateToken(claims);
    }

    public string GenerateToken(IEnumerable<Claim> claims)
    {
        var credentials = new SigningCredentials(_jwtOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            expires: _jwtOptions.ExpiryDate,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
