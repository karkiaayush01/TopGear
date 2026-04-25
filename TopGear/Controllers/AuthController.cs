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
    [AllowAnonymous]
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

    /// <summary>
    /// Login any user
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO request)
    {
        LoginResponseDTO? response = await authService.Login(request);

        if (response == null) return Unauthorized("Invalid Credentials");

        return Ok(response);
    }
}
