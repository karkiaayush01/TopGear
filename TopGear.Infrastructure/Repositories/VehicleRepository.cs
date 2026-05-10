using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class VehicleRepository(AppDbContext context) : RepositoryBase<Vehicle>(context), IVehicleRepository
{
    public async Task<List<Vehicle>> GetByCustomerIdAsync(Guid customerId) =>
        await Context.Vehicles
            .AsNoTracking()
            .Where(v => v.CustomerId == customerId)
            .ToListAsync();
}
