using TopGear.Application.DTOs.PartsDTO;
using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IPartRepository : IRepositoryBase<Part>
{
    Task<(List<Part> Parts, int TotalCount)> SearchPartsAsync(PartSearchQueryDTO query);
}
