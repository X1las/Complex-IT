using DataServiceLayer;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace WebServiceLayer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenLocalhost(5001); // HTTP only - no HTTPS certificate needed
        });

        // Register all your data services for dependency injection
        builder.Services.AddScoped<UserHistoryDataService>(); // Add this first for dependencies
        builder.Services.AddScoped<UserDataService>();
        builder.Services.AddScoped<TitleDataService>();
        builder.Services.AddScoped<CrewDataService>();
        builder.Services.AddScoped<BookmarkDataService>();
        builder.Services.AddScoped<UserRatingDataService>();
        // Add Mapster for object mapping
        builder.Services.AddMapster();
        
        // Add controllers
        builder.Services.AddControllers();

        // Add CORS if needed for frontend
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.UseCors("AllowAll");
        app.MapControllers();

        app.Run();
    }
}