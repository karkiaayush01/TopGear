using Microsoft.EntityFrameworkCore;
using TopGear.Application.DTOs.PartsDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class PartRepository(AppDbContext context): RepositoryBase<Part>(context), IPartRepository
{
    public async Task<(List<Part> Parts, int TotalCount)> SearchPartsAsync(PartSearchQueryDTO query)
    {
        var partsQuery = Context.Set<Part>()
            .Include(p => p.Vendor)
            .AsNoTracking()
            .AsQueryable();

        var searchTerm = string.IsNullOrWhiteSpace(query.Search) ? query.Q : query.Search;
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var normalizedSearch = searchTerm.Trim().ToLower();
            partsQuery = partsQuery.Where(p =>
                p.PartName.ToLower().Contains(normalizedSearch) ||
                p.Description.ToLower().Contains(normalizedSearch));
        }

        if (query.VehicleType.HasValue)
        {
            partsQuery = partsQuery.Where(p => p.VehicleType == query.VehicleType.Value);
        }

        if (query.MinSellingPrice.HasValue)
        {
            partsQuery = partsQuery.Where(p => p.SellingPrice >= query.MinSellingPrice.Value);
        }

        if (query.MaxSellingPrice.HasValue)
        {
            partsQuery = partsQuery.Where(p => p.SellingPrice <= query.MaxSellingPrice.Value);
        }

        if (query.MinQuantity.HasValue)
        {
            partsQuery = partsQuery.Where(p => p.Quantity >= query.MinQuantity.Value);
        }

        if (query.MaxQuantity.HasValue)
        {
            partsQuery = partsQuery.Where(p => p.Quantity <= query.MaxQuantity.Value);
        }

        var totalCount = await partsQuery.CountAsync();
        var parts = await partsQuery
            .OrderBy(p => p.PartName)
            .Take(query.Limit)
            .ToListAsync();

        return (parts, totalCount);
    }
}
