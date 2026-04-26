using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TopGear.Domain.Enums;

namespace TopGear.Domain.Entities;

public class Review
{
    public Guid ReviewId { get; set; } = Guid.NewGuid();

    public Guid CustomerId { get; set; }

    public ReviewType ReviewType { get; set; }

    public Guid? PartId { get; set; }

    public Guid? AppointmentId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; } = null!;

    [ForeignKey(nameof(PartId))]
    public Part? Part { get; set; }

    [ForeignKey(nameof(AppointmentId))]
    public ServiceAppointment? Appointment { get; set; }
}
