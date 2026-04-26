using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

public class ServiceAppointment
{
    [Key]
    public Guid AppointmentId { get; set; } = Guid.NewGuid();

    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public VehicleType VehicleType { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;
}
