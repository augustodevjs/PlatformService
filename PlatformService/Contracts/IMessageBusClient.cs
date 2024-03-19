using PlatformService.Dtos;

namespace PlatformService.Contracts;

public interface IMessageBusClient
{
    void PublishNewPlatform(PlatformPublishedDto platformPublished);
}
