using AutoMapper;
using PlatformService.Dtos;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Contracts;
using PlatformService.Entities;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPlatformRepository _repository;
    private readonly IMessageBusClient _messageBusClient;
    private readonly ILogger<PlatformsController> _logger;

    public PlatformsController(
        IMapper mapper,
        IPlatformRepository repository,
        IMessageBusClient messageBusClient,
        ILogger<PlatformsController> logger
    )
    {
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
        _messageBusClient = messageBusClient;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlaforms()
    {
        _logger.LogInformation("Getting Platforms...");

        var platforms = _repository.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlaformById")]
    public ActionResult<PlatformReadDto> GetPlaformById(int id)
    {
        _logger.LogInformation("Get Platform...");

        var platform = _repository.GetPlatformById(id);

        if (platform == null)
        {
            return NoContent();
        }

        return Ok(_mapper.Map<PlatformReadDto>(platform));
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        _logger.LogInformation("Create Platform...");

        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

        try
        {
            var platformPushedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPushedDto.Event = "Platform_Published";

            _messageBusClient.PublishNewPlatform(platformPushedDto);
        }
        catch (Exception ex)
        {

            string message = $"--> Could not send asynchronously: {ex.Message}";

            _logger.LogError(message);
        }

        return CreatedAtRoute(nameof(GetPlaformById), new { platformReadDto.Id }, platformReadDto);
    }
}
