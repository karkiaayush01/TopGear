using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using TopGear.Application.DTOs.PartsDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;

namespace TopGear.Application.Services;

public class PartService: IPartService
{
    private readonly IPartRepository _repository;
    private readonly IImageUploadService _imageUploadService;
    private readonly ILogger<PartService> _logger;
    public PartService(IPartRepository repository, ILogger<PartService> logger, IImageUploadService imageUploadService) 
    {
        _repository = repository;
        _logger = logger;
        _imageUploadService = imageUploadService;
    }
    public async Task<IEnumerable<PartDTO>> GetPartsAsync()
    {
        _logger.LogInformation("Fetching all parts");

        var parts = await _repository. FindAllAsync();

        return parts.Select(p => new PartDTO
        {
            PartId = p.PartId,
            PurchasePrice = p.PartName,
            PartPrice = p.PurchasePrice,
            SellingPrice = p.SellingPrice,
            Quantity = p.Quantity,
            VendorId = p.VendorId,
            VendorName = p.Vendor?.VendorName ?? "",
            Description = p.Description,
            VehicleType = p.VehicleType,
            ImageUrl = p.ImageUrl
        });
    }

    public async Task<PartDTO> GetPartByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching part with ID: {PartId}", id);

        var part = await _repository.GetByIdAsync(id);

        if (part == null)
        {
            _logger.LogWarning("Part not found with ID: {PartId}", id);
            return null!;
        }

        return new PartDTO
        {
            PartId = part.PartId,
            PurchasePrice = part.PartName,
            PartPrice = part.PurchasePrice,
            SellingPrice = part.SellingPrice,
            Quantity = part.Quantity,
            VendorId = part.VendorId,
            VendorName = part.Vendor?.VendorName ?? "",
            Description = part.Description,
            VehicleType = part.VehicleType,
            ImageUrl = part.ImageUrl
        };
    }

    public async Task<PartDTO> CreatePartAsync(CreatePartDTO dto)
    {
        try
        {
            _logger.LogInformation("Checking if part has image");

            string partImageUrl = "";
            if (dto.PartImage != null)
            {
                _logger.LogInformation("Image found, uploading to cloud storage");

                using var stream = dto.PartImage.OpenReadStream();

                partImageUrl = await _imageUploadService.UploadAsync(
                    stream,
                    dto.PartImage.FileName,
                    "TopGear/Parts"
                );
            }

            _logger.LogInformation("Creating new part: {PartName}", dto.PartName);

            var newPart = new Part
            {
                PartName = dto.PartName,
                PurchasePrice = dto.PartPrice,
                SellingPrice = dto.SellingPrice,
                Quantity = dto.Quantity,
                VendorId = dto.VendorId,
                Description = dto.Description,
                VehicleType = dto.VehicleType,
                ImageUrl = partImageUrl,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repository.Create(newPart);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Part created successfully with ID: {PartId}", newPart.PartId);

            return new PartDTO
            {
                PartId = newPart.PartId,
                PurchasePrice = newPart.PartName,
                PartPrice = newPart.PurchasePrice,
                SellingPrice = newPart.SellingPrice,
                Quantity = newPart.Quantity,
                VendorId = newPart.VendorId,
                Description = newPart.Description,
                VehicleType = newPart.VehicleType,
                ImageUrl = newPart.ImageUrl
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating part");
            throw;
        }
    }

    public async Task<PartDTO?> UpdatePartAsync(Guid id, EditPartDTO dto)
    {
        _logger.LogInformation("Updating part with ID: {PartId}", id);

        var part = await _repository.GetByIdAsync(id);

        if (part == null)
        {
            _logger.LogWarning("Update failed. Part not found with ID: {PartId}", id);
            return null;
        }

        part.PartName = string.IsNullOrWhiteSpace(dto.PartName) ? part.PartName : dto.PartName;
        part.PurchasePrice = dto.PartPrice ?? part.PurchasePrice;
        part.VendorId = dto.VendorId ?? part.VendorId;
        part.Description = string.IsNullOrWhiteSpace(dto.Description) ? part.Description : dto.Description;
        part.VehicleType = dto.VehicleType ?? part.VehicleType;
        part.ImageUrl = string.IsNullOrWhiteSpace(dto.ImageUrl) ? part.ImageUrl : dto.ImageUrl;
        part.UpdatedAt = DateTime.UtcNow;

        _repository.Update(part);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Part updated successfully with ID: {PartId}", id);

        return new PartDTO
        {
            PartId = part.PartId,
            PurchasePrice = part.PartName,
            PartPrice = part.PurchasePrice,
            SellingPrice = part.SellingPrice,
            VendorId = part.VendorId,
            Description = part.Description,
            VehicleType = part.VehicleType,
            ImageUrl = part.ImageUrl
        };
    }
    public async Task<bool> DeletePartAsync(Guid id)
    {
        _logger.LogInformation("Deleting part with ID: {PartId}", id);

        var part = await _repository.GetByIdAsync(id);

        if (part == null)
        {
            _logger.LogWarning("Delete failed. Part not found with ID: {PartId}", id);
            return false;
        }

        _repository.Delete(part);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Part deleted successfully with ID: {PartId}", id);

        return true;
    }
}
