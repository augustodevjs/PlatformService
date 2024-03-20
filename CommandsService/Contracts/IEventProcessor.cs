namespace CommandsService.Contracts;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}
