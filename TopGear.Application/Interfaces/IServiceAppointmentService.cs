using TopGear.Application.DTOs.AppointmentDTO;

namespace TopGear.Application.Interfaces;

public interface IServiceAppointmentService
{
    Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync();
    Task<AppointmentDTO?> GetAppointmentByIdAsync(Guid id);
    Task<IEnumerable<AppointmentDTO>> GetAppointmentsByCustomerAsync(Guid customerId);
    Task<AppointmentDTO> CreateAppointmentAsync(CreateAppointmentDTO dto);
    Task<AppointmentDTO?> UpdateAppointmentAsync(Guid id, UpdateAppointmentDTO dto);
    Task<bool> CancelAppointmentAsync(Guid id, Guid requestingUserId);
    Task<bool> DeleteAppointmentAsync(Guid id);
}
