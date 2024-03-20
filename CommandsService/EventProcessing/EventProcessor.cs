using AutoMapper;
using System.Text.Json;
using CommandsService.Dtos;
using CommandsService.Enum;
using CommandsService.Models;
using CommandsService.Contracts;

namespace CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IMapper _mapper;
    private readonly ILogger<EventProcessor> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public EventProcessor(
        IMapper mapper, 
        ILogger<EventProcessor> logger,
        IServiceScopeFactory scopeFactory
    )
    {
        _logger = logger;
        _mapper = mapper;
        _scopeFactory = scopeFactory;
    }

    public void ProcessEvent(string message)
    {
        var eventType = DetermineEvent(message);

        switch (eventType)
        {
            case EventType.PlatformPublished:
                addPlatform(message);
                break;
            default:
                break;
        }
    }

    private EventType DetermineEvent(string notifcationMessage)
    {
        _logger.LogInformation("--> Determining Event");

        var eventType = JsonSerializer.Deserialize<GenericEventDto>(notifcationMessage);

        switch (eventType!.Event)
        {
            case "Platform_Published":
                _logger.LogInformation("--> Platform Published Event Detected");
                return EventType.PlatformPublished;
            default:
                _logger.LogInformation("--> Could not determine the event type");
                return EventType.Undetermined;
        }
    }

    private void addPlatform(string platformPublishedMessage)
    {
        using var scope = _scopeFactory.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<ICommandRepository>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
            var platform = _mapper.Map<Platform>(platformPublishedDto);
            if (!repository.ExternalPlatformExist(platform.ExternalID))
            {
                repository.CreatePlatform(platform);
                repository.SaveChanges();

                _logger.LogInformation("--> Platform added!");
            }
            else
            {
                _logger.LogInformation("--> Platform already exisits...");
            }

        }
        catch (Exception ex)
        {
            string message = $"--> Could not add Platform to DB {ex.Message}";

            _logger.LogError(message);
        }
    }
}
