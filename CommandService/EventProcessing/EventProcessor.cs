using System.Text.Json;
using AutoMapper;
using CommandService.Data.Repositories;
using CommandService.Dtos;
using CommandService.Models;

namespace CommandService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
        _scopeFactory = scopeFactory;
        _mapper = mapper;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublish:
                AddPlatform(message);
                break;
            default:
                break;
        }
    }

    private void AddPlatform(string platformPublishMessage)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var repo = scope.ServiceProvider
                .GetRequiredService<ICommandRepo>();
            var platformPublishDto = JsonSerializer
                .Deserialize<PlatformPublishDto>(platformPublishMessage);
            try
            {
                var platform = _mapper
                    .Map<PlatformModel>(platformPublishDto);
                bool isPlatformExists = repo.IsExternalPlatformExists(platform.ExternalId);

                if (isPlatformExists)
                {
                    Console.WriteLine("--> Platform already exists!");
                    return;
                }
                repo.CreatePlatform(platform);
                repo.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add Platform to Db {ex.Message}");
            }
        }
    }

    private EventType DetermineEvent(string notificationMsg)
    {
        Console.WriteLine("--> Determining Event...");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMsg);

        switch (eventType?.Event)
        {
            case "Platform_Publish":
                Console.WriteLine("Platform_Publish Event detected.");
                return EventType.PlatformPublish;
            default:
                Console.WriteLine("--> Could not determine event type.");
                return EventType.Undetermined;
        }
    }
}

enum EventType
{
    PlatformPublish,
    Undetermined
}