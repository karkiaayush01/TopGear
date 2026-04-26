namespace TopGear.Infrastructure.Config;

public class EmailSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; } = 587;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}
