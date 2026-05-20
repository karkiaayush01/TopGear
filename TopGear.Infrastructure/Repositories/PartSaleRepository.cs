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

    public async Task<List<PartSale>> GetAllWithDetailsAsync() =>
        await Context.PartSales
            .Include(s => s.Customer)
            .Include(s => s.CreatedBy)
            .Include(s => s.Items)
                .ThenInclude(i => i.Part)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();

    public async Task<List<PartSale>> GetSalesForReportAsync(DateTime? from, DateTime? to)
    {
        var query = Context.PartSales
            .Include(s => s.Customer)
            .Include(s => s.Items)
                .ThenInclude(i => i.Part)
            .AsNoTracking();

        if (from.HasValue)
            query = query.Where(s => s.SaleDate >= from.Value);

        if (to.HasValue)
            query = query.Where(s => s.SaleDate <= to.Value);

        return await query.OrderBy(s => s.SaleDate).ToListAsync();
    }
}
