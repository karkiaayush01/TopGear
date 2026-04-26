using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IReviewRepository : IRepositoryBase<Review>
{
    Task<List<Review>> GetAllWithCustomerAsync();
    Task<Review?> GetByIdWithCustomerAsync(Guid id);
    Task<List<Review>> GetByPartIdAsync(Guid partId);
    Task<List<Review>> GetByAppointmentIdAsync(Guid appointmentId);
    Task<List<Review>> GetByCustomerIdAsync(Guid customerId);
}
