using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class ServiceAppointmentRepository(AppDbContext context)
    : RepositoryBase<ServiceAppointment>(context), IServiceAppointmentRepository
{
    public async Task<List<ServiceAppointment>> GetAllWithCustomerAsync()
    {
        return await Context.Set<ServiceAppointment>()
            .Include(a => a.Customer)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<ServiceAppointment?> GetByIdWithCustomerAsync(Guid id)
    {
        return await Context.Set<ServiceAppointment>()
            .Include(a => a.Customer)
            .FirstOrDefaultAsync(a => a.AppointmentId == id);
    }

    public async Task<List<ServiceAppointment>> GetByCustomerIdAsync(Guid customerId)
    {
        return await Context.Set<ServiceAppointment>()
            .Include(a => a.Customer)
            .Where(a => a.CustomerId == customerId)
            .AsNoTracking()
            .ToListAsync();
    }
}
