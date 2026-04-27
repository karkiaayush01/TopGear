namespace TopGear.Application.Interfaces;

public interface IStaffService
{
    Task DeactivateStaff(Guid staffId);

    Task DeleteStaff(Guid staffId);
}
