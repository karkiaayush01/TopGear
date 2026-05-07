using TopGear.Application.DTOs.PartRequestDTO;

namespace TopGear.Application.Interfaces;

public interface IPartRequestService
{
    Task<IEnumerable<PartRequestDTO>> GetAllRequestsAsync();
    Task<PartRequestDTO?> GetRequestByIdAsync(Guid id);
    Task<IEnumerable<PartRequestDTO>> GetRequestsByCustomerAsync(Guid customerId);
    Task<PartRequestDTO> CreateRequestAsync(Guid customerId, CreatePartRequestDTO dto);
    Task<PartRequestDTO?> ReviewRequestAsync(Guid id, ReviewPartRequestDTO dto);
}
