using TopGear.Application.DTOs.UserDTO;

namespace TopGear.Application.Interfaces;

public interface IStaffService
{
    Task<IEnumerable<StaffDTO>> GetAllStaff(bool includeDeleted = false);

    Task DeactivateStaff(Guid staffId);

    Task ActivateStaff(Guid staffId);

    Task DeleteStaff(Guid staffId);
}
