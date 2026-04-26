using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class PartRepository(AppDbContext context): RepositoryBase<Part>(context), IPartRepository
{
}
