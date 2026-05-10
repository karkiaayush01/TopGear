using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IVehicleRepository : IRepositoryBase<Vehicle>
{
    Task<List<Vehicle>> GetByCustomerIdAsync(Guid customerId);
}
