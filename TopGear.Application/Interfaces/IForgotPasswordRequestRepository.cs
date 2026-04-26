using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IForgotPasswordRequestRepository : IRepositoryBase<ForgotPasswordRequest>
{
    Task InvalidateAllOldRequests(string email);
}
