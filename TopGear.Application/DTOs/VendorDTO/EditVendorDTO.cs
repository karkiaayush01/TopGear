
using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.VendorDTO;

public class EditVendorDTO
{
    [Required]
    public Guid VendorId { get; set; }

    [Required]
    [MaxLength(100)]
    public string VendorName { get; set; } = string.Empty;

    [MaxLength(150)]
    public string? CompanyName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    [MaxLength(250)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? ContactPerson { get; set; }

    public bool IsActive { get; set; }
}
