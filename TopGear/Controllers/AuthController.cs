using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.UserDTO;

namespace TopGear.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Self Register Endpoint for a customer
    /// </summary>
    [HttpPost("register")]
    public async Task SelfRegisterCustomer([FromBody] RegisterDTO data)
    {

    }
}
