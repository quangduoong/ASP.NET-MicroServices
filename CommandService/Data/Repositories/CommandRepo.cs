using CommandService.Models;

namespace CommandService.Data.Repositories;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }

    public void CreateCommand(int platformId, CommandModel command)
    {
        if (command is null)
            throw new ArgumentNullException(nameof(command));
        command.PlatformId = platformId;
        _context.Add(command);
    }

    public void CreatePlatform(PlatformModel platform)
    {
        if (platform is null)
            throw new ArgumentNullException(nameof(platform));
        _context.Add(platform);
    }

    public IEnumerable<PlatformModel> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public CommandModel GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .FirstOrDefault(c => c.PlatformId == platformId && c.Id == commandId)
            ?? default!;
    }

    public IEnumerable<CommandModel> GetCommandsForPlatform(int platformId)
    {
        return _context.Commands.Where(c => c.PlatformId == platformId);
    }

    public bool IsExternalPlatformExists(int externalPlatformId)
    {
        return _context.Platforms.Any(p => p.ExternalId == externalPlatformId);
    }

    public bool IsPlatformExists(int platformId)
    {
        return _context.Platforms.Any(p => p.Id == platformId);
    }

    public bool SaveChanges()
    {
        return _context.SaveChanges() >= 0;
    }
}