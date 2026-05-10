using Microsoft.EntityFrameworkCore;
using TopGear.Application.DTOs.Common;
using TopGear.Application.DTOs.CustomerDTO;
using TopGear.Application.Interfaces;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext context) : ICustomerRepository
{
    public async Task<PagedResult<CustomerResponse>> SearchAsync(CustomerSearchParams parameters)
    {
        var query = from user in context.AppUsers
                    join userRole in context.UserRoles on user.Id equals userRole.UserId
                    join role in context.Roles on userRole.RoleId equals role.Id
                    where role.Name == "Customer"
                    select user;

        if (!string.IsNullOrWhiteSpace(parameters.Name))
        {
            var name = parameters.Name.ToLower();
            query = query.Where(u =>
                (u.FirstName + " " + u.LastName).ToLower().Contains(name) ||
                u.FirstName.ToLower().Contains(name) ||
                u.LastName.ToLower().Contains(name));
        }

        if (!string.IsNullOrWhiteSpace(parameters.Phone))
            query = query.Where(u => u.PhoneNumber != null && u.PhoneNumber.Contains(parameters.Phone));

        if (parameters.CustomerId.HasValue)
            query = query.Where(u => u.Id == parameters.CustomerId.Value);

        if (!string.IsNullOrWhiteSpace(parameters.VehiclePlateNumber))
        {
            var plate = parameters.VehiclePlateNumber.ToLower();
            var matchingCustomerIds = context.Vehicles
                .Where(v => v.PlateNumber.ToLower().Contains(plate))
                .Select(v => v.CustomerId);
            query = query.Where(u => matchingCustomerIds.Contains(u.Id));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .Select(u => new CustomerResponse
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                PhoneNumber = u.PhoneNumber,
                ImageUrl = u.ImageUrl
            })
            .ToListAsync();

        return new PagedResult<CustomerResponse>
        {
            Items = items,
            TotalCount = totalCount,
            Page = parameters.Page,
            PageSize = parameters.PageSize
        };
    }
}
