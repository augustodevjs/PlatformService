using System.Text;
using System.Text.Json;
using PlatformService.Dtos;
using PlatformService.Contracts;

namespace PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpCommandDataClient> _logger;

    public HttpCommandDataClient(HttpClient httpClient, ILogger<HttpCommandDataClient> logger, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform),
            Encoding.UTF8,
            "application/json"
        );

        var commandService = $"{_configuration["CommandService"]}/c/Platforms";

        var response = await _httpClient.PostAsync($"{commandService}", httpContent);

        if(response.IsSuccessStatusCode)
        {
            _logger.LogInformation($"--> Sync POST to URL CommandService: {commandService} was OK");
        } else
        {
            _logger.LogInformation($"--> Sync POST to {commandService} was NOT OK");
        }
    }
}
