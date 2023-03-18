using CommandService.Models;

namespace CommandService.SyncDataServices.Grpc;

public interface IPlatformDataClient
{
    IEnumerable<PlatformModel> ReturnAllPlatforms();
}