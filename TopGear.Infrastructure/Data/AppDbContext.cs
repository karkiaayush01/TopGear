using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TopGear.Domain.Entities;

namespace TopGear.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options): IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<User> AppUsers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("topgear");
    }
}
