using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class Seeds
{
    public static Boolean PlatformSeeder(this ModelBuilder builder, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment()) return false;

        Console.WriteLine("--> Seeding platforms (Development Env)...");

        builder.Entity<PlatformModel>().HasData(
            new PlatformModel
            {
                Id = 1,
                Name = "Dot Net",
                Publisher = "Microsoft",
                Cost = "Free"
            },
            new PlatformModel
            {
                Id = 2, 
                Name = "SQL Server Express",
                Publisher = "Microsoft",
                Cost = "Free"
            },
            new PlatformModel
            {
                Id = 3,
                Name = "Kubernetes",
                Publisher = "Cloud Native Computing Foundation",
                Cost = "Free"
            }
        );

        return true;
    }

    public static Boolean PlatformSeeder(IServiceProvider serviceProvider)
    {
        Console.WriteLine("--> Seeding platforms data (on runtime)...");

        using (var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>()))
        {
            if (context.Platforms.Any())
                return false;

            context.Platforms.AddRange(
                new PlatformModel
                {
                    Name = "Dot Net",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new PlatformModel
                {
                    Name = "SQL Server Express",
                    Publisher = "Microsoft",
                    Cost = "Free"
                },
                new PlatformModel
                {
                    Name = "Kubernetes",
                    Publisher = "Cloud Native Computing Foundation",
                    Cost = "Free"
                }
            );

            context.SaveChanges();
        }

        return true;
    }
}