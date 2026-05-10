using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

public class Vehicle
{
    [Key]
    public Guid VehicleId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [StringLength(100)]
    public string Make { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Model { get; set; } = null!;

    [Range(1900, 2100)]
    public int Year { get; set; }

    [Required]
    [StringLength(20)]
    public string PlateNumber { get; set; } = null!;

    public VehicleType VehicleType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;
}
