using CommandsService.Events;
using CommandsService.Context;
using CommandsService.Contracts;
using Microsoft.EntityFrameworkCore;
using CommandsService.Infraestructure.Messaging;
using CommandsService.Infraestructure.Persistancy;

namespace CommandsService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHostedService<MessageBusSubscriber>();
        builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
        builder.Services.AddScoped<ICommandRepository, CommandRepository>();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        builder.Services.AddDbContext<AppDbContext>(
            options =>options.UseInMemoryDatabase("InMem")
        );

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
