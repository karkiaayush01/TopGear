using TopGear.Application.DTOs.VehicleDTO;

namespace TopGear.Application.Interfaces;

public interface IVehicleService
{
    Task<List<VehicleDTO>> GetCustomerVehiclesAsync(Guid customerId);
    Task<VehicleDTO> AddVehicleAsync(Guid customerId, CreateVehicleDTO dto);
    Task<VehicleDTO> UpdateVehicleAsync(Guid customerId, Guid vehicleId, UpdateVehicleDTO dto);
    Task DeleteVehicleAsync(Guid customerId, Guid vehicleId);
}
