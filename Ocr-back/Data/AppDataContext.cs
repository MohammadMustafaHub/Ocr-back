using Microsoft.EntityFrameworkCore;
using Ocr_back.Models;

namespace Ocr_back.Data;

public class AppDataContext : DbContext
{
    public AppDataContext(DbContextOptions<AppDataContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // modelBuilder.Entity<ImageProcess>()
        //     .HasIndex();
    }

    public DbSet<ImageProcess> ImageProcesses { get; set; }
}