using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class ReviewRepository(AppDbContext context)
    : RepositoryBase<Review>(context), IReviewRepository
{
    public async Task<List<Review>> GetAllWithCustomerAsync()
    {
        return await Context.Set<Review>()
            .Include(r => r.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Review?> GetByIdWithCustomerAsync(Guid id)
    {
        return await Context.Set<Review>()
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.ReviewId == id);
    }

    public async Task<List<Review>> GetByPartIdAsync(Guid partId)
    {
        return await Context.Set<Review>()
            .Include(r => r.Customer)
            .Where(r => r.PartId == partId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Review>> GetByAppointmentIdAsync(Guid appointmentId)
    {
        return await Context.Set<Review>()
            .Include(r => r.Customer)
            .Where(r => r.AppointmentId == appointmentId)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Review>> GetByCustomerIdAsync(Guid customerId)
    {
        return await Context.Set<Review>()
            .Include(r => r.Customer)
            .Where(r => r.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync();
    }
}
