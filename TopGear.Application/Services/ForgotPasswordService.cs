using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using TopGear.Application.DTOs.ForgotPasswordDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;
using TopGear.Domain.Enums;

namespace TopGear.Application.Services;

public class ForgotPasswordService : IForgotPasswordService
{
    private readonly IForgotPasswordRequestRepository _repository;
    private readonly ILogger<ForgotPasswordService> _logger;
    private readonly UserManager<User> _userManager;

    public ForgotPasswordService(
        IForgotPasswordRequestRepository repository,
        ILogger<ForgotPasswordService> logger,
        UserManager<User> userManager)
    {
        _repository = repository;
        _logger = logger;
        _userManager = userManager;
    }

    public async Task<ForgotPasswordRequestDTO> CreateRequest(string email)
    {
        _logger.LogInformation("Invalidating all old requests for {Email}", email);
        await _repository.InvalidateAllOldRequests(email);

        _logger.LogInformation("Generating verification code");
        string code = GenerateCode();

        var request = new ForgotPasswordRequest
        {
            UserEmail = email,
            VerificationCode = code,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            Status = ForgotPasswordStatus.Pending
        };

        _repository.Create(request);
        await _repository.SaveChangesAsync();

        return new ForgotPasswordRequestDTO
        {
            RequestId = request.RequestId,
            UserEmail = request.UserEmail,
            VerificationCode = request.VerificationCode,
            CreatedAt = request.CreatedAt,
            ExpiresAt = request.ExpiresAt,
            Status = request.Status
        };
    }

    public async Task ResetPassword(ResetPasswordDTO data)
    {
        _logger.LogInformation("Processing reset password for {Email}", data.Email);

        var request = await _repository
            .FindByCondition(r =>
                r.UserEmail == data.Email &&
                r.Status == ForgotPasswordStatus.Pending &&
                r.ExpiresAt > DateTime.UtcNow)
            .FirstOrDefaultAsync();

        if (request == null)
            throw new ArgumentException("Invalid or expired reset request.");

        if (request.VerificationCode != data.VerificationCode)
            throw new ArgumentException("Verification code is incorrect.");

        var user = await _userManager.FindByEmailAsync(data.Email);

        if (user == null)
            throw new ArgumentException("User not found.");

        _logger.LogInformation("Resetting password for {Email}", data.Email);

        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        var result = await _userManager.ResetPasswordAsync(user, resetToken, data.Password);

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        request.Status = ForgotPasswordStatus.Success;
        await _repository.SaveChangesAsync();
    }

    private string GenerateCode()
    {
        int code = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return code.ToString("D6");
    }
}
