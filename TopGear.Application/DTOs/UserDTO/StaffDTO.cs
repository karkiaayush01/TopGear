using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.UserDTO;

public class StaffDTO
{
    public Guid UserId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public UserAccountStatus Status { get; set; }
}
