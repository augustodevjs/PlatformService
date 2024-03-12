using PlatformService.Data;
using PlatformService.Contracts;
using PlatformService.Repository;
using Microsoft.EntityFrameworkCore;
using PlatformService.SyncDataServices.Http;

namespace PlatformService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
        builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();

        builder.Services.AddDbContext<AppDbContext>(
            options => options.UseInMemoryDatabase("InMem"));

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        var app = builder.Build();
        var logger = app.Services.GetRequiredService<ILogger<Program>>();

        PrepDb.PrepPopulation(app);

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
