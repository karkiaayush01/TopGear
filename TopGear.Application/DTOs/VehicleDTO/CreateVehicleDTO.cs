using System.ComponentModel.DataAnnotations;
using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.VehicleDTO;

public class CreateVehicleDTO
{
    [Required]
    [StringLength(100)]
    public string Make { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = null!;

    [Required]
    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [StringLength(20)]
    public string PlateNumber { get; set; } = null!;

    [Required]
    public VehicleType VehicleType { get; set; }
}
