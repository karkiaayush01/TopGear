using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

public class User: IdentityUser<Guid>
{
    [StringLength(255)]
    public string FirstName { get; set; } = null!;

    [StringLength(255)]
    public string LastName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public UserAccountStatus Status { get; set; } = UserAccountStatus.Inactive;
}
