using AutoMapper;
using PlatformService.Dtos;
using PlatformService.Models;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Contracts;

namespace PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IPlatformRepository _repository;
    private readonly ILogger<PlatformsController> _logger;

    public PlatformsController(
        IMapper mapper, 
        IPlatformRepository repository, 
        ILogger<PlatformsController> logger
        )
    {
        _mapper = mapper;
        _logger = logger;
        _repository = repository;
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
    public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        _logger.LogInformation("Create Platform...");

        var platformModel = _mapper.Map<Platform>(platformCreateDto);
        _repository.CreatePlatform(platformModel);
        _repository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

        return CreatedAtRoute(nameof(GetPlaformById), new { platformReadDto.Id }, platformReadDto);
    }
}
