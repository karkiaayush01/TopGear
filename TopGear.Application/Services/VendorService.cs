using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.VendorDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;

namespace TopGear.Application.Services;

public class VendorService : IVendorService
{
    private readonly IVendorRepository _repository;
    private readonly ILogger<VendorService> _logger;

    public VendorService(IVendorRepository repository, ILogger<VendorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<VendorDTO>> GetVendorAsync()
    {
        _logger.LogInformation("Fetching all vendors");

        var vendors = await _repository.FindAllAsync();

        return vendors.Select(v => new VendorDTO
        {
            VendorId = v.VendorId,
            VendorName = v.VendorName,
            CompanyName = v.CompanyName,
            Email = v.Email,
            Phone = v.Phone,
            Address = v.Address,
            ContactPerson = v.ContactPerson,
            IsActive = v.IsActive,
            CreatedAt = v.CreatedAt
        });
    }

    public async Task<VendorDTO> GetVendorByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching vendor with ID: {VendorId}", id);

        var vendor = await _repository.GetByIdAsync(id);

        if (vendor == null)
        {
            _logger.LogWarning("Vendor not found with ID: {VendorId}", id);
            return null!;
        }

        return new VendorDTO
        {
            VendorId = vendor.VendorId,
            VendorName = vendor.VendorName,
            CompanyName = vendor.CompanyName,
            Email = vendor.Email,
            Phone = vendor.Phone,
            Address = vendor.Address,
            ContactPerson = vendor.ContactPerson,
            IsActive = vendor.IsActive,
            CreatedAt = vendor.CreatedAt
        };
    }

    public async Task<VendorDTO> CreateVendorAsync(CreateVendorDTO dto)
    {
        try
        {
            _logger.LogInformation("Creating new vendor: {VendorName}", dto.VendorName);

            var newVendor = new Vendor
            {
                VendorName = dto.VendorName,
                CompanyName = dto.CompanyName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                ContactPerson = dto.ContactPerson,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repository.Create(newVendor);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Vendor created successfully with ID: {VendorId}", newVendor.VendorId);

            return new VendorDTO
            {
                VendorId = newVendor.VendorId,
                VendorName = newVendor.VendorName,
                CompanyName = newVendor.CompanyName,
                Email = newVendor.Email,
                Phone = newVendor.Phone,
                Address = newVendor.Address,
                ContactPerson = newVendor.ContactPerson,
                IsActive = newVendor.IsActive,
                CreatedAt = newVendor.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating vendor");
            throw;
        }
    }

    public async Task<VendorDTO?> UpdateVendorAsync(Guid id, EditVendorDTO dto)
    {
        _logger.LogInformation("Updating vendor with ID: {VendorId}", id);

        var vendor = await _repository.GetByIdAsync(id);

        if (vendor == null)
        {
            _logger.LogWarning("Update failed. Vendor not found with ID: {VendorId}", id);
            return null;
        }

        vendor.VendorName = string.IsNullOrWhiteSpace(dto.VendorName) ? vendor.VendorName : dto.VendorName;
        vendor.CompanyName = string.IsNullOrWhiteSpace(dto.CompanyName) ? vendor.CompanyName : dto.CompanyName;
        vendor.Email = string.IsNullOrWhiteSpace(dto.Email) ? vendor.Email : dto.Email;
        vendor.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? vendor.Phone : dto.Phone;
        vendor.Address = string.IsNullOrWhiteSpace(dto.Address) ? vendor.Address : dto.Address;
        vendor.ContactPerson = string.IsNullOrWhiteSpace(dto.ContactPerson) ? vendor.ContactPerson : dto.ContactPerson;
        vendor.IsActive = dto.IsActive;
        vendor.UpdatedAt = DateTime.UtcNow;

        _repository.Update(vendor);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Vendor updated successfully with ID: {VendorId}", id);

        return new VendorDTO
        {
            VendorId = vendor.VendorId,
            VendorName = vendor.VendorName,
            CompanyName = vendor.CompanyName,
            Email = vendor.Email,
            Phone = vendor.Phone,
            Address = vendor.Address,
            ContactPerson = vendor.ContactPerson,
            IsActive = vendor.IsActive,
            CreatedAt = vendor.CreatedAt
        };
    }

    public async Task<bool> DeleteVendorAsync(Guid id)
    {
        _logger.LogInformation("Deleting vendor with ID: {VendorId}", id);

        var vendor = await _repository.GetByIdAsync(id);

        if (vendor == null)
        {
            _logger.LogWarning("Delete failed. Vendor not found with ID: {VendorId}", id);
            return false;
        }

        _repository.Delete(vendor);
        await _repository.SaveChangesAsync();

        _logger.LogInformation("Vendor deleted successfully with ID: {VendorId}", id);

        return true;
    }
}