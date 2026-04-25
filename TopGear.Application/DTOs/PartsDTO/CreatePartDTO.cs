using System.ComponentModel.DataAnnotations;
using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.PartsDTO;

public class CreatePartDTO
{
    [Required]
    public string PartName { get; set; } = null!;

    [Range(0, double.MaxValue)]
    public decimal PartPrice { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [Required]
    public Guid VendorId { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public VehicleType VehicleType { get; set; }

    [Required]
    public string ImageUrl { get; set; } = null!;

}
