using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.PartRequestDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;

namespace TopGear.Application.Services;

public class PartRequestService : IPartRequestService
{
    private readonly IPartRequestRepository _repository;
    private readonly ILogger<PartRequestService> _logger;

    public PartRequestService(IPartRequestRepository repository, ILogger<PartRequestService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<PartRequestDTO>> GetAllRequestsAsync()
    {
        _logger.LogInformation("Fetching all part requests");

        var requests = await _repository.GetAllWithCustomerAsync();
        return requests.Select(MapToDTO);
    }

    public async Task<PartRequestDTO?> GetRequestByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching part request with ID: {PartRequestId}", id);

        var request = await _repository.GetByIdWithCustomerAsync(id);
        return request == null ? null : MapToDTO(request);
    }

    public async Task<IEnumerable<PartRequestDTO>> GetRequestsByCustomerAsync(Guid customerId)
    {
        _logger.LogInformation("Fetching part requests for customer: {CustomerId}", customerId);

        var requests = await _repository.GetByCustomerIdAsync(customerId);
        return requests.Select(MapToDTO);
    }

    public async Task<PartRequestDTO> CreateRequestAsync(Guid customerId, CreatePartRequestDTO dto)
    {
        _logger.LogInformation("Creating part request for customer: {CustomerId}", customerId);

        var request = new PartRequest
        {
            CustomerId = customerId,
            PartName = dto.PartName,
            VehicleDetails = dto.VehicleDetails,
            Quantity = dto.Quantity,
            Notes = dto.Notes,
            Status = PartRequestStatus.Pending,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _repository.Create(request);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Part request created successfully with ID: {PartRequestId}", request.PartRequestId);

        var createdRequest = await _repository.GetByIdWithCustomerAsync(request.PartRequestId);
        return MapToDTO(createdRequest ?? request);
    }

    public async Task<PartRequestDTO?> ReviewRequestAsync(Guid id, ReviewPartRequestDTO dto)
    {
        _logger.LogInformation("Reviewing part request with ID: {PartRequestId}", id);

        var request = await _repository.GetByIdWithCustomerAsync(id);

        if (request == null)
        {
            _logger.LogWarning("Review failed. Part request not found with ID: {PartRequestId}", id);
            return null;
        }

        request.Status = dto.Status;
        request.AdminNotes = dto.AdminNotes;
        request.UpdatedAt = DateTime.UtcNow;

        _repository.Update(request);
        await _repository.SaveChangesAsync();

        return MapToDTO(request);
    }

    private static PartRequestDTO MapToDTO(PartRequest request)
    {
        return new PartRequestDTO
        {
            PartRequestId = request.PartRequestId,
            CustomerId = request.CustomerId,
            CustomerName = request.Customer != null ? $"{request.Customer.FirstName} {request.Customer.LastName}" : "",
            PartName = request.PartName,
            VehicleDetails = request.VehicleDetails,
            Quantity = request.Quantity,
            Notes = request.Notes,
            Status = request.Status,
            AdminNotes = request.AdminNotes,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt
        };
    }
}
