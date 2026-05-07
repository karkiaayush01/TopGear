using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IPartRequestRepository : IRepositoryBase<PartRequest>
{
    Task<List<PartRequest>> GetAllWithCustomerAsync();
    Task<PartRequest?> GetByIdWithCustomerAsync(Guid id);
    Task<List<PartRequest>> GetByCustomerIdAsync(Guid customerId);
}
