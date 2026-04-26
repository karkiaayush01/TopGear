using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;

namespace TopGear.Application.Services;

public class UserService: IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly ILogger<AuthService> _logger;

    public UserService(UserManager<User> userManager, ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<bool> CheckUserExistsByEmail(string email)
    {
        _logger.LogInformation("Finding user by email: {email}", email);

        return await _userManager.FindByEmailAsync(email) is not null;
    }
}
