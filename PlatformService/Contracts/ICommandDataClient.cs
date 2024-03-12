using PlatformService.Dtos;

namespace PlatformService.Contracts;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
