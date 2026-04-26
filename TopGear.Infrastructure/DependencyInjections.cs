using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopGear.Application.Interfaces;
using TopGear.Application.Services;
using TopGear.Infrastructure.Data;
using TopGear.Infrastructure.Repositories;

namespace TopGear.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        services.AddScoped<AppDbSeeder>();

        //repository injections
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IServiceAppointmentRepository, ServiceAppointmentRepository>();

        //services injections
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPartService, PartService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IServiceAppointmentService, ServiceAppointmentService>();

        return services;
    }
}
