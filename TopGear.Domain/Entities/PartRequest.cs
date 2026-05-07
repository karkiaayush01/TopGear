using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

public class PartRequest
{
    [Key]
    public Guid PartRequestId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    [MaxLength(150)]
    public string PartName { get; set; } = null!;

    [MaxLength(250)]
    public string? VehicleDetails { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public string? Notes { get; set; }

    public PartRequestStatus Status { get; set; } = PartRequestStatus.Pending;

    public string? AdminNotes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;
}
