using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IServiceAppointmentRepository : IRepositoryBase<ServiceAppointment>
{
    Task<List<ServiceAppointment>> GetAllWithCustomerAsync();
    Task<ServiceAppointment?> GetByIdWithCustomerAsync(Guid id);
    Task<List<ServiceAppointment>> GetByCustomerIdAsync(Guid customerId);
}
