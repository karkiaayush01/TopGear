using TopGear.Application.DTOs.UserDTO;

namespace TopGear.Application.Interfaces;

public interface IAuthService
{
    public Task<Guid> CreateAccount(RegisterDTO data, string role);
}
