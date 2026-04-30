using TopGear.Application.DTOs.UserDTO;

namespace TopGear.Application.Interfaces;

public interface IAuthService
{
    Task<Guid> CreateAccount(RegisterDTO data, string role);

    Task<LoginResponseDTO?> Login(LoginDTO data);

    Task<UserDTO> GetAuthenticatedUserData(string userId);
}
