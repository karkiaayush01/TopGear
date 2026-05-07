using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class PartRequestRepository(AppDbContext context)
    : RepositoryBase<PartRequest>(context), IPartRequestRepository
{
    public async Task<List<PartRequest>> GetAllWithCustomerAsync()
    {
        return await Context.Set<PartRequest>()
            .Include(r => r.Customer)
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<PartRequest?> GetByIdWithCustomerAsync(Guid id)
    {
        return await Context.Set<PartRequest>()
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.PartRequestId == id);
    }

    public async Task<List<PartRequest>> GetByCustomerIdAsync(Guid customerId)
    {
        return await Context.Set<PartRequest>()
            .Include(r => r.Customer)
            .Where(r => r.CustomerId == customerId)
            .AsNoTracking()
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}
