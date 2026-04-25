using Microsoft.AspNetCore.Identity;
using TopGear.Domain.Entities;
using TopGear.Infrastructure;
using TopGear.Infrastructure.Data;
using TopGear.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<GlobalExceptionHandler>();

builder.Services.AddDataProtection();

builder.Services
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// The code below seeds the required data for the application in the db. Uncomment the code and start the application when using a new database that doesn't have these values stored.
// Seeds Roles for the application

/*
using var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<AppDbSeeder>();
await seeder.SeedRolesAsync();
*/

app.Run();
