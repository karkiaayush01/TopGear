using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopGear.Application.Interfaces;
using TopGear.Application.Services;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        services.AddScoped<AppDbSeeder>();

        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
