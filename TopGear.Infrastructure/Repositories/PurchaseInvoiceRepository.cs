using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class PurchaseInvoiceRepository(AppDbContext context)
    : RepositoryBase<PurchaseInvoice>(context), IPurchaseInvoiceRepository
{
    public async Task<PurchaseInvoice?> GetByIdWithDetailsAsync(Guid id)
    {
        return await Context.Set <PurchaseInvoice>()
            .Include(x => x.Vendor)
            .Include(x => x.Items)
                .ThenInclude(i => i.Part)
            .FirstOrDefaultAsync(x => x.PurchaseInvoiceId == id);
    }

    public async Task<IEnumerable<PurchaseInvoice>> GetAllWithDetailsAsync()
    {
        return await Context.Set<PurchaseInvoice>()
            .Include(x => x.Vendor)
            .Include(x => x.Items)
                .ThenInclude(i => i.Part)
            .ToListAsync();
    }

    public async Task<List<PurchaseInvoice>> FindAllWithDetailsAsync(bool trackChanges = false)
    {
        var query = Context.PurchaseInvoices
            .Include(x => x.Vendor)
            .Include(x => x.Items)
                .ThenInclude(i => i.Part);

        return !trackChanges
            ? await query.AsNoTracking().ToListAsync()
            : await query.ToListAsync();
    }

    public async Task<List<PurchaseInvoice>> GetForReportAsync(DateTime? from, DateTime? to)
    {
        var query = Context.PurchaseInvoices
            .Include(x => x.Vendor)
            .Include(x => x.Items)
                .ThenInclude(i => i.Part)
            .AsNoTracking();

        if (from.HasValue)
            query = query.Where(x => x.InvoiceDate >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.InvoiceDate <= to.Value);

        return await query.ToListAsync();
    }
}