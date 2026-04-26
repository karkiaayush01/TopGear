using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.AppointmentDTO;

public class UpdateAppointmentDTO
{
    public AppointmentStatus? Status { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public string? Notes { get; set; }
}
