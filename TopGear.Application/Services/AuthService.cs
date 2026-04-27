using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.UserDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;

namespace TopGear.Application.Services;

public class AuthService: IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(UserManager<User> userManager, ILogger<AuthService> logger, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _logger = logger;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Guid> CreateAccount(RegisterDTO data, string role)
    {
        _logger.LogInformation("Creating new user object");

        var user = new User
        {
            FirstName = data.FirstName,
            LastName = data.LastName,
            UserName = data.Email,
            Email = data.Email,
            PhoneNumber = data.PhoneNumber,
            Status = UserAccountStatus.Active
        };

        _logger.LogInformation("Adding {rule} {email} with User Manager", role, user.Email);

        var result = await _userManager.CreateAsync(user, data.Password);

        if (!result.Succeeded) {
            _logger.LogWarning("User creation failed for {Email}. Errors: {Errors}",
                data.Email,
                result.Errors.Select(e => e.Description)
            );

            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));

            throw new ArgumentException($"User creation failed: {errorMessage}");
        };

        _logger.LogInformation("Created User {UserId}! Now Creating Roles", user.Id);
        var roleResult = await _userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
        {
            _logger.LogWarning("Role creation failed. Errors: {Errors}",
                roleResult.Errors.Select(e => e.Description)
            );

            var errorMessage = string.Join("; ", roleResult.Errors.Select(e => e.Description));

            throw new ArgumentException($"Role assignment failed: {errorMessage}");
        };

        return user.Id;
    }

    public async Task<LoginResponseDTO?> Login(LoginDTO request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return null;

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid) return null;

        var token = await _jwtTokenService.GenerateUserToken(user);

        return new LoginResponseDTO
        {
            AccessToken = token,
            UserId = user.Id,
            Email = user.Email ?? "",
            Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? ""
        };
    }
}
