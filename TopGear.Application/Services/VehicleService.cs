using Microsoft.AspNetCore.Identity;
using TopGear.Application.CustomExceptions;
using TopGear.Application.DTOs.VehicleDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;

namespace TopGear.Application.Services;

public class VehicleService(IVehicleRepository vehicleRepository, UserManager<User> userManager) : IVehicleService
{
    public async Task<List<VehicleDTO>> GetCustomerVehiclesAsync(Guid customerId)
    {
        await EnsureCustomerExistsAsync(customerId);
        var vehicles = await vehicleRepository.GetByCustomerIdAsync(customerId);
        return vehicles.Select(MapToDTO).ToList();
    }

    public async Task<VehicleDTO> AddVehicleAsync(Guid customerId, CreateVehicleDTO dto)
    {
        await EnsureCustomerExistsAsync(customerId);

        var vehicle = new Vehicle
        {
            CustomerId = customerId,
            Make = dto.Make,
            Model = dto.Model,
            Year = dto.Year,
            PlateNumber = dto.PlateNumber,
            VehicleType = dto.VehicleType
        };

        vehicleRepository.Create(vehicle);
        await vehicleRepository.SaveChangesAsync();

        return MapToDTO(vehicle);
    }

    public async Task<VehicleDTO> UpdateVehicleAsync(Guid customerId, Guid vehicleId, UpdateVehicleDTO dto)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(vehicleId)
            ?? throw new NotFoundException("Vehicle not found.");

        if (vehicle.CustomerId != customerId)
            throw new BadRequestException("Vehicle does not belong to this customer.");

        vehicle.Make = dto.Make ?? vehicle.Make;
        vehicle.Model = dto.Model ?? vehicle.Model;
        vehicle.Year = dto.Year ?? vehicle.Year;
        vehicle.PlateNumber = dto.PlateNumber ?? vehicle.PlateNumber;
        vehicle.VehicleType = dto.VehicleType ?? vehicle.VehicleType;
        vehicle.UpdatedAt = DateTime.UtcNow;

        vehicleRepository.Update(vehicle);
        await vehicleRepository.SaveChangesAsync();

        return MapToDTO(vehicle);
    }

    public async Task DeleteVehicleAsync(Guid customerId, Guid vehicleId)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(vehicleId)
            ?? throw new NotFoundException("Vehicle not found.");

        if (vehicle.CustomerId != customerId)
            throw new BadRequestException("Vehicle does not belong to this customer.");

        vehicleRepository.Delete(vehicle);
        await vehicleRepository.SaveChangesAsync();
    }

    private async Task EnsureCustomerExistsAsync(Guid customerId)
    {
        var user = await userManager.FindByIdAsync(customerId.ToString())
            ?? throw new NotFoundException("Customer not found.");

        var roles = await userManager.GetRolesAsync(user);
        if (!roles.Contains("Customer"))
            throw new BadRequestException("User is not a customer.");
    }

    private static VehicleDTO MapToDTO(Vehicle v) => new()
    {
        VehicleId = v.VehicleId,
        CustomerId = v.CustomerId,
        Make = v.Make,
        Model = v.Model,
        Year = v.Year,
        PlateNumber = v.PlateNumber,
        VehicleType = v.VehicleType
    };
}
