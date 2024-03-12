using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ILogger<PlatformsController> _logger;

    public PlatformsController(ILogger<PlatformsController> logger)
    {
        _logger = logger;
    }

    [HttpPost]
    public ActionResult TestInBoundConnection()
    {
        _logger.LogInformation("--> inbound Post # Command Service");

        return Ok("Inbound test of from Platforms Controller");
    }
}
