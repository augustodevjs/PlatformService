using PlatformService.Data;
using Microsoft.EntityFrameworkCore;
using PlatformService.Contracts;
using PlatformService.Repository;

namespace PlatformService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddScoped<IPlatformRepository, PlatformRepository>();

        builder.Services.AddDbContext<AppDbContext>(options => 
        options.UseInMemoryDatabase("InMem"));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
