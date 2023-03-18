using CommandService.Data.Repositories;
using CommandService.Models;
using CommandService.SyncDataServices.Grpc;

namespace CommandService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>() ?? default!;
            var platforms = grpcClient.ReturnAllPlatforms() ?? default!;
            var commandRepo = serviceScope.ServiceProvider.GetService<ICommandRepo>() ?? default!;

            PS_SeedData(commandRepo, platforms);
        }
    }

    private static void PS_SeedData(ICommandRepo repo, IEnumerable<PlatformModel> platforms)
    {
        Console.WriteLine("Seeding new platforms...");

        foreach (var platform in platforms)
        {
            if (!repo.IsExternalPlatformExists(platform.ExternalId))
            {
                repo.CreatePlatform(platform);
            }
            repo.SaveChanges();
        }
    }
}