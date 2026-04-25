using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IJwtTokenService
{
    Task<String> GenerateUserToken(User user);
}
