using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TopGear.Application.Interfaces;
using TopGear.Application.Services;
using TopGear.Infrastructure.Config;
using TopGear.Infrastructure.Data;
using TopGear.Infrastructure.Email;
using TopGear.Infrastructure.Repositories;

namespace TopGear.Infrastructure;

public static class DependencyInjections
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Postgres")));
        services.AddScoped<AppDbSeeder>();

        // Email Service
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

        //repository injections
        services.AddScoped<IPartRepository, PartRepository>();
        services.AddScoped<IVendorRepository, VendorRepository>();
        services.AddScoped<IServiceAppointmentRepository, ServiceAppointmentRepository>();
        services.AddScoped<IPurchaseInvoiceRepository, PurchaseInvoiceRepository>();
        services.AddScoped<IPurchaseInvoiceItemRepository, PurchaseInvoiceItemRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IForgotPasswordRequestRepository, ForgotPasswordRequestRepository>();

        //services injections
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IForgotPasswordService, ForgotPasswordService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPartService, PartService>();
        services.AddScoped<IVendorService, VendorService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IServiceAppointmentService, ServiceAppointmentService>();
        services.AddScoped<IPurchaseInvoiceService, PurchaseInvoiceService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IStaffService, StaffService>();

        return services;
    }
}
