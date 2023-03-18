using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using PlatformService.Models;

namespace PlatformService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        var env = this.GetService<IWebHostEnvironment>();
        bool isSeed = false;

        base.OnModelCreating(builder);
        isSeed = builder.PlatformSeeder(env);
    }

    public DbSet<PlatformModel> Platforms { get; set; } = default!;
}