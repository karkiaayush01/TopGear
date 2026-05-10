using TopGear.Application.DTOs.Common;
using TopGear.Application.DTOs.CustomerDTO;

namespace TopGear.Application.Interfaces;

public interface ICustomerRepository
{
    Task<PagedResult<CustomerResponse>> SearchAsync(CustomerSearchParams parameters);
}
