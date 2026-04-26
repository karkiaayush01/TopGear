using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.AppointmentDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;

namespace TopGear.Application.Services;

public class ServiceAppointmentService : IServiceAppointmentService
{
    private readonly IServiceAppointmentRepository _repository;
    private readonly ILogger<ServiceAppointmentService> _logger;

    public ServiceAppointmentService(IServiceAppointmentRepository repository, ILogger<ServiceAppointmentService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<AppointmentDTO>> GetAllAppointmentsAsync()
    {
        _logger.LogInformation("Fetching all service appointments");

        var appointments = await _repository.GetAllWithCustomerAsync();

        return appointments.Select(a => MapToDTO(a));
    }

    public async Task<AppointmentDTO?> GetAppointmentByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching appointment with ID: {AppointmentId}", id);

        var appointment = await _repository.GetByIdWithCustomerAsync(id);

        if (appointment == null)
        {
            _logger.LogWarning("Appointment not found with ID: {AppointmentId}", id);
            return null;
        }

        return MapToDTO(appointment);
    }

    public async Task<IEnumerable<AppointmentDTO>> GetAppointmentsByCustomerAsync(Guid customerId)
    {
        _logger.LogInformation("Fetching appointments for customer: {CustomerId}", customerId);

        var appointments = await _repository.GetByCustomerIdAsync(customerId);

        return appointments.Select(a => MapToDTO(a));
    }

    public async Task<AppointmentDTO> CreateAppointmentAsync(CreateAppointmentDTO dto)
    {
        _logger.LogInformation("Creating new appointment for customer: {CustomerId}", dto.CustomerId);

        var appointment = new ServiceAppointment
        {
            CustomerId = dto.CustomerId,
            VehicleType = dto.VehicleType,
            AppointmentDate = dto.AppointmentDate,
            Notes = dto.Notes,
            Status = AppointmentStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repository.Create(appointment);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Appointment created successfully with ID: {AppointmentId}", appointment.AppointmentId);

        return MapToDTO(appointment);
    }

    public async Task<AppointmentDTO?> UpdateAppointmentAsync(Guid id, UpdateAppointmentDTO dto)
    {
        _logger.LogInformation("Updating appointment with ID: {AppointmentId}", id);

        var appointment = await _repository.GetByIdWithCustomerAsync(id);

        if (appointment == null)
        {
            _logger.LogWarning("Update failed. Appointment not found with ID: {AppointmentId}", id);
            return null;
        }

        appointment.Status = dto.Status ?? appointment.Status;
        appointment.AppointmentDate = dto.AppointmentDate ?? appointment.AppointmentDate;
        appointment.Notes = dto.Notes ?? appointment.Notes;
        appointment.UpdatedAt = DateTime.UtcNow;

        _repository.Update(appointment);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Appointment updated successfully with ID: {AppointmentId}", id);

        return MapToDTO(appointment);
    }

    public async Task<bool> CancelAppointmentAsync(Guid id, Guid requestingUserId)
    {
        _logger.LogInformation("Customer {UserId} attempting to cancel appointment {AppointmentId}", requestingUserId, id);

        var appointment = await _repository.GetByIdAsync(id);

        if (appointment == null)
        {
            _logger.LogWarning("Cancel failed. Appointment not found with ID: {AppointmentId}", id);
            return false;
        }

        if (appointment.CustomerId != requestingUserId)
        {
            _logger.LogWarning("Customer {UserId} tried to cancel appointment {AppointmentId} that does not belong to them", requestingUserId, id);
            throw new UnauthorizedAccessException("You can only cancel your own appointments.");
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.UpdatedAt = DateTime.UtcNow;

        _repository.Update(appointment);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Appointment {AppointmentId} cancelled by customer {UserId}", id, requestingUserId);

        return true;
    }

    public async Task<bool> DeleteAppointmentAsync(Guid id)
    {
        _logger.LogInformation("Deleting appointment with ID: {AppointmentId}", id);

        var appointment = await _repository.GetByIdAsync(id);

        if (appointment == null)
        {
            _logger.LogWarning("Delete failed. Appointment not found with ID: {AppointmentId}", id);
            return false;
        }

        _repository.Delete(appointment);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Appointment deleted successfully with ID: {AppointmentId}", id);

        return true;
    }

    // maps entity to DTO, avoids repeating the same code in every method
    private static AppointmentDTO MapToDTO(ServiceAppointment a)
    {
        return new AppointmentDTO
        {
            AppointmentId = a.AppointmentId,
            CustomerId = a.CustomerId,
            CustomerName = a.Customer != null ? $"{a.Customer.FirstName} {a.Customer.LastName}" : "",
            VehicleType = a.VehicleType,
            AppointmentDate = a.AppointmentDate,
            Status = a.Status,
            Notes = a.Notes,
            CreatedAt = a.CreatedAt,
            UpdatedAt = a.UpdatedAt
        };
    }
}
