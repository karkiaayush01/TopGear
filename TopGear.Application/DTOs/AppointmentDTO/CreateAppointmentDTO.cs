using System.ComponentModel.DataAnnotations;
using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.AppointmentDTO;

public class CreateAppointmentDTO
{
    [Required]
    public Guid CustomerId { get; set; }

    [Required]
    public VehicleType VehicleType { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    public string? Notes { get; set; }
}
