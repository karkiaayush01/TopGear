using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.UserDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/staff")]
public class StaffController: ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IStaffService _staffService;

    public StaffController(IAuthService authService, IStaffService staffService)
    {
        _authService = authService;
        _staffService = staffService;
    }


    /// <summary>
    /// Register a staff. Requires admin previleges
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpPost("/register")]
    public async Task<IActionResult> RegisterStaff(RegisterDTO request)
    {
        Guid userId = await _authService.CreateAccount(request, "Staff");

        return Ok(new
        {
            Message = "Staff registered successfully",
            UserId = userId
        });
    }

    /// <summary>
    /// Deactivate a staff account.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeactivateStaff(Guid staffId)
    {
        await _staffService.DeactivateStaff(staffId);

        // Return Ok directly. If there's any issue, services will throw an exception
        return Ok("The account has been deactivated successfully");
    }

    /// <summary>
    /// Soft-delete a staff account
    /// </summary>
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteStaff(Guid staffId)
    {
        await _staffService.DeleteStaff(staffId);

        return Ok("Staff has been deleted");
    }
}
