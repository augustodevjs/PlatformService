using PlatformService.Data;
using Microsoft.OpenApi.Models;
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

        if (builder.Environment.IsProduction())
        {
            Console.WriteLine("Using SqlServerDb");

            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConnection")
            ));
        }
        else
        {
            Console.WriteLine("Using inMemDb");

            builder.Services.AddDbContext<AppDbContext>(
                options => options.UseInMemoryDatabase("InMem")
            );
        }

        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
        builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddControllers();
        builder.Services.AddAuthorization();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "PlatformService", Version = "v1" });
        });

        var app = builder.Build();

        PrepDb.PrepPopulation(app, builder.Environment.IsProduction());

        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "PlatformService v1");
        });

        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
