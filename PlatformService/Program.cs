using PlatformService.Context;
using Microsoft.OpenApi.Models;
using PlatformService.Contracts;
using Microsoft.EntityFrameworkCore;
using PlatformService.Infraestructure.Messaging;
using PlatformService.Infraestructure.Persistancy.Repository;

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

        builder.Services.AddSingleton<IMessageBusClient, RabbitMQService>();
        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();
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
