using AutoMapper;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;
using CommandsService.Contracts;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICommandRepository _repository;
    private readonly ILogger<PlatformsController> _logger;

    public PlatformsController(
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
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        _logger.LogInformation("--> Getting Platforms from CommandsService");

        var platformItems = _repository.GetAllPlatforms();

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformItems));
    }

    [HttpPost]
    public ActionResult TestInBoundConnection()
    {
        _logger.LogInformation("--> inbound Post # Command Service");

        return Ok("Inbound test of from Platforms Controller");
    }
}
