using System.ComponentModel.DataAnnotations;
using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.VehicleDTO;

public class UpdateVehicleDTO
{
    [StringLength(100)]
    public string? Make { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    [Range(1900, 2100)]
    public int? Year { get; set; }

    [StringLength(20)]
    public string? PlateNumber { get; set; }

    public VehicleType? VehicleType { get; set; }
}
