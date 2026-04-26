using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IPurchaseInvoiceRepository : IRepositoryBase<PurchaseInvoice>
{
    Task<PurchaseInvoice?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<PurchaseInvoice>> GetAllWithDetailsAsync();
    Task<List<PurchaseInvoice>> FindAllWithDetailsAsync(bool trackChanges = false);

}
