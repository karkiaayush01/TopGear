using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TopGear.Domain.Entities;

public class User: IdentityUser<Guid>
{
    [StringLength(255)]
    public string FirstName { get; set; } = null!;

    [StringLength(255)]
    public string LastName { get; set; } = null!;
}
