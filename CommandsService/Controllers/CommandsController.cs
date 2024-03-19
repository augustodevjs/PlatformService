using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;
using CommandsService.Contracts;

namespace CommandsService.Controllers;

[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICommandRepository _repository;
    private readonly ILogger<PlatformsController> _logger;

    public CommandsController(
        IMapper mapper, 
        ICommandRepository repository, 
        ILogger<PlatformsController> logger
    )
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
    }

    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        _logger.LogInformation($"Hit GetCommandsForPlatform: {platformId}");

        if(!_repository.PlatformExist(platformId))
        {
            return NotFound();
        }

        var commands = _repository.GetCommandsForPlatform(platformId);

        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }

    [HttpGet("{commandId}", Name = "GetCommandForPlatfortm")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        _logger.LogInformation($"Hit GetCommandForPlatfortm: {platformId} / {commandId}");

        if (!_repository.PlatformExist(platformId))
        {
            return NotFound();
        }

        var command = _repository.GetCommand(platformId, commandId);

        if(command == null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<CommandReadDto>(command));
    }

    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        _logger.LogInformation($"Hit CreateCommandForPlatform: {platformId}");

        if (!_repository.PlatformExist(platformId))
        {
            return NotFound();
        }

        var command = _mapper.Map<Command>(commandDto);

        _repository.CreateCommand(platformId, command);
        _repository.SaveChanges();

        var commandReadDto = _mapper.Map<CommandReadDto>(command);

        return CreatedAtRoute(nameof(GetCommandForPlatform), new { platformId, commandId = commandReadDto.Id }, commandReadDto);
    }
}
