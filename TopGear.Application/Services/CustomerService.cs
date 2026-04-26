using Microsoft.AspNetCore.Identity;
using TopGear.Application.DTOs.CustomerDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;

namespace TopGear.Application.Services;

public class CustomerService(UserManager<User> userManager) : ICustomerService
{
    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.Phone
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, "Customer");

        return MapToResponse(user);
    }

    public async Task<CustomerResponse> GetByIdAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException("Customer not found.");

        return MapToResponse(user);
    }

    public async Task UpdateAsync(Guid id, UpdateCustomerRequest request)
    {
        var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException("Customer not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.UserName = request.Email;
        user.PhoneNumber = request.Phone;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }

    public async Task DeactivateAsync(Guid id)
    {
        var user = await userManager.FindByIdAsync(id.ToString())
            ?? throw new KeyNotFoundException("Customer not found.");

        user.LockoutEnabled = true;
        user.LockoutEnd = DateTimeOffset.MaxValue;

        await userManager.UpdateAsync(user);
    }

    private static CustomerResponse MapToResponse(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email!,
        PhoneNumber = user.PhoneNumber,
        ImageUrl = user.ImageUrl
    };
}