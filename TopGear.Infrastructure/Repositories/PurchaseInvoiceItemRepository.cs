using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class PurchaseInvoiceItemRepository(AppDbContext context): RepositoryBase<PurchaseInvoiceItem>(context), IPurchaseInvoiceItemRepository
{
}
