using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.UserDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;

namespace TopGear.Application.Services;

public class StaffService : IStaffService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<StaffService> _logger;

    public StaffService(UserManager<User> userManager, ILogger<StaffService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<IEnumerable<StaffDTO>> GetAllStaff(bool includeDeleted = false)
    {
        _logger.LogInformation("Listing staff (includeDeleted={includeDeleted})", includeDeleted);

        var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");

        var filtered = includeDeleted
            ? staffUsers
            : staffUsers.Where(u => u.Status != UserAccountStatus.Deleted);

        return filtered
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new StaffDTO
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                FullName = $"{u.FirstName} {u.LastName}".Trim(),
                Email = u.Email ?? string.Empty,
                PhoneNumber = u.PhoneNumber,
                Status = u.Status,
            })
            .ToList();
    }

    public async Task DeactivateStaff(Guid staffId)
    {
        _logger.LogInformation("Deactivating staff account with id {staffId}", staffId);

        User? staff = await _userManager.FindByIdAsync(staffId.ToString()) ?? throw new KeyNotFoundException("Staff not found");
        if (staff.Status != UserAccountStatus.Active) throw new ArgumentException("This account is already deactivated");

        var staffRole = (await _userManager.GetRolesAsync(staff)).FirstOrDefault();
        if (staffRole != "Staff") throw new ArgumentException("Not a staff account");

        staff.Status = UserAccountStatus.Deactivated;

        _logger.LogInformation("Updating staff account status");
        var result = await _userManager.UpdateAsync(staff);

        if (!result.Succeeded)
        {
            _logger.LogError("An error occurred while updating staff status");
            throw new Exception("Failed to update staff status");
        }
    }

    public async Task ActivateStaff(Guid staffId)
    {
        _logger.LogInformation("Activating staff account with id {staffId}", staffId);

        User? staff = await _userManager.FindByIdAsync(staffId.ToString()) ?? throw new KeyNotFoundException("Staff not found");
        if (staff.Status == UserAccountStatus.Active) throw new ArgumentException("This account is already active");
        if (staff.Status == UserAccountStatus.Deleted) throw new ArgumentException("Cannot activate a deleted account");

        var staffRole = (await _userManager.GetRolesAsync(staff)).FirstOrDefault();
        if (staffRole != "Staff") throw new ArgumentException("Not a staff account");

        staff.Status = UserAccountStatus.Active;

        _logger.LogInformation("Updating staff account status");
        var result = await _userManager.UpdateAsync(staff);

        if (!result.Succeeded)
        {
            _logger.LogError("An error occurred while updating staff status");
            throw new Exception("Failed to update staff status");
        }
    }

    public async Task DeleteStaff(Guid staffId)
    {
        _logger.LogInformation("Deleting staff account with id {staffId}", staffId);

        User? staff = await _userManager.FindByIdAsync(staffId.ToString()) ?? throw new KeyNotFoundException("Staff not found");

        var staffRole = (await _userManager.GetRolesAsync(staff)).FirstOrDefault();
        if (staffRole != "Staff") throw new ArgumentException("Not a staff account");

        staff.Status = UserAccountStatus.Deleted;

        _logger.LogInformation("Updating staff account status");
        var result = await _userManager.UpdateAsync(staff);

        if (!result.Succeeded)
        {
            _logger.LogError("An error occurred while updating staff status");
            throw new Exception("Failed to update staff status");
        }
    }
}
