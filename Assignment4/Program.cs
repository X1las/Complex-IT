using DataServiceLayer;
using Microsoft.AspNetCore.Builder;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace WebServiceLayer;

public class Program
{
    public static DataService DataService { get; } = new DataService();
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Kestrel to listen on port 5001
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5001);
        });

        // Add services to the container.
        builder.Services.AddSingleton<DataService>();
        builder.Services.AddMapster();
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapControllers();

        app.Run();
    }
}