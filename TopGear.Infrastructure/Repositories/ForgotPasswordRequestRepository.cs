using TopGear.Domain.Entities;
using TopGear.Infrastructure.Data;
using TopGear.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using TopGear.Application.Interfaces;

namespace TopGear.Infrastructure.Repositories;

public class ForgotPasswordRequestRepository(AppDbContext context): RepositoryBase<ForgotPasswordRequest>(context), IForgotPasswordRequestRepository
{
    public async Task InvalidateAllOldRequests(string email)
    {
        var pendingRequests = await Context.Set<ForgotPasswordRequest>().Where(r => r.Status == ForgotPasswordStatus.Pending).ToListAsync();

        foreach (var request in pendingRequests)
        {
            request.Status = ForgotPasswordStatus.Expired;
        }

        await Context.SaveChangesAsync();
    }
}
