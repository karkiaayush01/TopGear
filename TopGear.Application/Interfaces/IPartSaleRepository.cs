using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IPartSaleRepository : IRepositoryBase<PartSale>
{
    Task<PartSale?> GetByIdWithDetailsAsync(Guid saleId);
    Task<List<PartSale>> GetByCustomerIdAsync(Guid customerId);
}
