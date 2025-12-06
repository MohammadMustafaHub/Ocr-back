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
        
        modelBuilder.Entity<AccessToken>().HasKey(x => x.Id);
        modelBuilder.Entity<AccessToken>().HasIndex(x => x.Token);
        modelBuilder.Entity<AccessToken>().Property(x => x.TokenType).HasConversion<string>();
    }

    public DbSet<AccessToken> AccessTokens { get; set; }
}