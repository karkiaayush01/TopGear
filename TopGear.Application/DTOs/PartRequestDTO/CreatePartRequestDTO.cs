using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.PartRequestDTO;

public class CreatePartRequestDTO
{
    [Required]
    [MaxLength(150)]
    public string PartName { get; set; } = null!;

    [MaxLength(250)]
    public string? VehicleDetails { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public string? Notes { get; set; }
}
