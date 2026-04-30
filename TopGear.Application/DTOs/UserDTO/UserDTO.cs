namespace TopGear.Application.DTOs.UserDTO;

public class UserDTO
{
    public Guid UserId { get; set; } = Guid.Empty;

    // Return combined fullname instead of first and last name
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; } = null;
}
