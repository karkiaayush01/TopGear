using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
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

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials(); // if using cookies/auth
    });
});

//Add Swagger too for quick testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Name = "Authorization",
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter JWT token only. Do not write Bearer manually."
    });

    options.OperationFilter<AuthorizeOperationFilter>();

    /*
     * Generate XML Documentation for API 
     * Referred: https://learn.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-8.0&tabs=visual-studio
    */
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});


// Jwt configuration
var jwtConfig = builder.Configuration.GetSection(JwtOptions.SectionName);
builder.Services.AddOptions<JwtOptions>()
    .Bind(jwtConfig)
    .ValidateDataAnnotations();

// Exception Handler
builder.Services.AddScoped<GlobalExceptionHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, CustomAuthorizationMiddlewareResultHandler>();

builder.Services.AddDataProtection();

builder.Services
    .AddIdentityCore<User>(options => options.User.RequireUniqueEmail = true) // Require unique email per user. Since we are sending emails, it has to be unique per user
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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

// Logging configuration: Log using Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning)
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Debug(outputTemplate:
        "[{Timestamp:HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseCors("AllowFrontend");
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionHandler>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
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

// Swagger UI: Add Lock options for Authorize Endpoints and leave for anonymous
public class AuthorizeOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasAuthorize = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any()
            || context.MethodInfo.DeclaringType!
            .GetCustomAttributes(true)
            .OfType<AuthorizeAttribute>()
            .Any();

        var hasAllowAnonymous = context.MethodInfo
            .GetCustomAttributes(true)
            .OfType<AllowAnonymousAttribute>()
            .Any();

        if (!hasAuthorize || hasAllowAnonymous)
        {
            // No lock for default endpoints with no authrize or anonymous
            operation.Security = new List<OpenApiSecurityRequirement>();
            return;
        }

        var requirement = new OpenApiSecurityRequirement();
        requirement.Add(new OpenApiSecuritySchemeReference("Bearer", context.Document), new List<string>());
        operation.Security = new List<OpenApiSecurityRequirement> { requirement };
    }
}
