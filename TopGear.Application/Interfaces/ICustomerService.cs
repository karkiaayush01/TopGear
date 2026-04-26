 using TopGear.Application.DTOs.CustomerDTO;


namespace TopGear.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
        Task<CustomerResponse> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UpdateCustomerRequest request);
        Task DeactivateAsync(Guid id);
    }
}
