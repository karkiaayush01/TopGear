using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.UserDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    /// <summary>
    /// Self Signup Endpoint for a customer
    /// </summary>
    [HttpPost("signup")]
    public async Task<IActionResult> SelfRegisterCustomer([FromBody] RegisterDTO data)
    {
        Guid userId = await authService.CreateAccount(data, "Customer");
        return Ok(new
        {
            Message = "Customer registered successfully",
            UserId = userId
        });
    }
}
