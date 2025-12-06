using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Ocr_back.Models;

namespace Ocr_back.Data;

public class AuthDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> op): base(op)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<RefreshToken>().HasKey(x => x.Id);
        modelBuilder.Entity<RefreshToken>().HasIndex(x => x.Token);
    }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
}