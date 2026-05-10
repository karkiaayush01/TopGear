using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class PartSaleRepository(AppDbContext context) : RepositoryBase<PartSale>(context), IPartSaleRepository
{
    public async Task<PartSale?> GetByIdWithDetailsAsync(Guid saleId) =>
        await Context.PartSales
            .Include(s => s.Customer)
            .Include(s => s.CreatedBy)
            .Include(s => s.Items)
                .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(s => s.SaleId == saleId);

    public async Task<List<PartSale>> GetByCustomerIdAsync(Guid customerId) =>
        await Context.PartSales
            .Include(s => s.Customer)
            .Include(s => s.CreatedBy)
            .Include(s => s.Items)
                .ThenInclude(i => i.Part)
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
}
