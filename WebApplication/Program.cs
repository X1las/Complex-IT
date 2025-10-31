using DataServiceLayer;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using WebServiceLayer.Utils;

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

        // Register DbContext
        builder.Services.AddDbContext<ImdbContext>();

        // Register all data services for dependency injection
        builder.Services.AddScoped<UserHistoryDataService>();
        builder.Services.AddScoped<UserDataService>();
        builder.Services.AddScoped<TitleDataService>();
        builder.Services.AddScoped<CrewDataService>();
        builder.Services.AddScoped<BookmarkDataService>();
        builder.Services.AddScoped<UserRatingDataService>();
        
        // Register utility services
        builder.Services.AddScoped<Hashing>();
        
        // Register Mapster for object mapping
        builder.Services.AddMapster();
        
        // Add controllers
        builder.Services.AddControllers();

        // Add CORS for frontend
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