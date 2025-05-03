using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using AuthImplementation.Domain.Entities;

namespace AuthImplementation.Infrastructure.Persistence;

public class AppDbContext : IdentityDbContext<AppUser>
{

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}