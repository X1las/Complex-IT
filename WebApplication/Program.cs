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
            options.ListenLocalhost(5001);
        });

        // Register all your data services for dependency injection
        builder.Services.AddScoped<UserDataService>();
        builder.Services.AddScoped<TitleDataService>();      // If you have this
        builder.Services.AddScoped<CrewDataService>();       // If you have this
        builder.Services.AddScoped<BookmarkDataService>();   // If you have this
        builder.Services.AddScoped<UserRatingDataService>();     // If you have this
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