namespace TopGear.Application.DTOs.UserDTO;

public class LoginResponseDTO
{
    public string AccessToken { get; set; } = null!;
    public Guid UserId { get; set; } = Guid.Empty;
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
}
