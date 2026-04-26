using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.AppointmentDTO;

public class AppointmentDTO
{
    public Guid AppointmentId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public VehicleType VehicleType { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
