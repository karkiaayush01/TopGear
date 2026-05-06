using TopGear.Application.DTOs.PartsDTO;

namespace TopGear.Application.Interfaces;

public interface IPartService
{
    Task<IEnumerable<PartDTO>> GetPartsAsync();
    Task<PagedPartsResponseDTO> SearchPartsAsync(PartSearchQueryDTO query);
    Task<PartDTO> GetPartByIdAsync(Guid id);
    Task<PartDTO> CreatePartAsync(CreatePartDTO partCreateDTO);
    Task<PartDTO?> UpdatePartAsync(Guid id, EditPartDTO partUpdateDTO);
    Task<bool> DeletePartAsync(Guid id);

}
