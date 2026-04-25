namespace TopGear.Application.DTOs.UserDTO;

public class RegisterDTO
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    
    // Phone number is optional
    public string? PhoneNumber { get; set; } = null;
}
