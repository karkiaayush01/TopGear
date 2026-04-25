using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using TopGear.Domain.Entities;
using TopGear.Infrastructure;
using TopGear.Infrastructure.Auth;
using TopGear.Infrastructure.Data;
using TopGear.Middleware;

// Preserves the claim as it came from token instead of Microsoft's Mapping Standards
JsonWebTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Jwt configuration
var jwtConfig = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.AddOptions<JwtOptions>()
    .Bind(jwtConfig)
    .ValidateDataAnnotations();

// Exception Handler
builder.Services.AddScoped<GlobalExceptionHandler>();

builder.Services.AddDataProtection();

builder.Services
    .AddIdentityCore<User>()
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authentication
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        var jwtOptions = jwtConfig.Get<JwtOptions>() ?? throw new InvalidOperationException("Jwt Configuration is missing");

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = jwtOptions.SymmetricSecurityKey,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

//Authorization
builder.Services.AddAuthorization();

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
