using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.EmailDTO;
using TopGear.Application.DTOs.ForgotPasswordDTO;
using TopGear.Application.DTOs.UserDTO;
using TopGear.Application.Interfaces;
using TopGear.Application.Utils;
using TopGear.Domain.Entities;

namespace TopGear.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IEmailService _emailService;
    private readonly IUserService _userService;
    private readonly IForgotPasswordService _forgotPasswordService;

    public AuthController(IAuthService authService, IEmailService emailService, IUserService userService, IForgotPasswordService forgotPasswordService)
    {
        _authService = authService;
        _emailService = emailService;
        _userService = userService;
        _forgotPasswordService = forgotPasswordService;
    }


    /// <summary>
    /// Self Signup Endpoint for a customer
    /// </summary>
    [AllowAnonymous]
    [HttpPost("signup")]
    public async Task<IActionResult> SelfRegisterCustomer([FromBody] RegisterDTO data)
    {
        Guid userId = await _authService.CreateAccount(data, "Customer");
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
        LoginResponseDTO? response = await _authService.Login(request);

        if (response == null) return Unauthorized("Invalid Credentials");

        return Ok(response);
    }


    /// <summary>
    /// Send Forgot Password Email
    /// </summary>
    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> SendForgotPasswordEmail(string email)
    {
        if (!ValidationUtils.IsValidEmail(email)) throw new ArgumentException("Invalid email. Please provide a valid email.");

        bool exists = await _userService.CheckUserExistsByEmail(email);

        if (exists)
        {
            var request = await _forgotPasswordService.CreateRequest(email);

            await _emailService.SendForgotPasswordEmail(email, request.VerificationCode);
        }

        return Ok(new
        {
            Message = "If the email is registered, an email with verification code has been sent"
        });
    }

    /// <summary>
    /// Reset a password using verification code
    /// </summary>
    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
    {
        await _forgotPasswordService.ResetPassword(request);

        return Ok("Password Reset Successfully");
    }

    /// <summary>
    /// Get information of the logged in user from the token.
    /// </summary>
    [Authorize]
    [HttpGet("user/me")]
    public async Task<IActionResult> GetAuthenticatedUserData()
    {
        var userId = User.FindFirst("sub")?.Value;

        if (userId == null) return BadRequest("Could not get userId. Please login first");

        var userData = await _authService.GetAuthenticatedUserData(userId);

        return Ok(userData);
    }
}
