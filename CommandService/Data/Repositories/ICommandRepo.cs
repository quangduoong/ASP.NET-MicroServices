using CommandService.Models;

namespace CommandService.Data.Repositories;

public interface ICommandRepo
{
    bool SaveChanges();
    // Platform
    IEnumerable<PlatformModel> GetAllPlatforms();
    void CreatePlatform(PlatformModel platform);
    bool IsPlatformExists(int platformId);
    bool IsExternalPlatformExists(int externalPlatformId);
    // Command
    IEnumerable<CommandModel> GetCommandsForPlatform(int platformId);
    CommandModel GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, CommandModel command);
}