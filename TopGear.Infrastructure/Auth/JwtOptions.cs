using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TopGear.Infrastructure.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Audience { get; init; } = null!;

    [Required]
    [MinLength(32)]
    public string Secret { get; init; } = null!;
    public SymmetricSecurityKey SymmetricSecurityKey => new (Encoding.UTF8.GetBytes(Secret));

    [Range(1, int.MaxValue)]
    public int ExpiryHours { get; init; }
    public DateTime ExpiryDate => DateTime.UtcNow.AddHours(ExpiryHours);
}
