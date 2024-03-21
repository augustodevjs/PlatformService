using CommandsService.Entities;

namespace CommandsService.Contracts;

public interface ICommandRepository
{
    bool SaveChanges();
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool PlatformExist(int platformId);
    bool ExternalPlatformExist(int externalPlatformId);
    IEnumerable<Command> GetCommandsForPlatform(int platformId);
    Command GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}
