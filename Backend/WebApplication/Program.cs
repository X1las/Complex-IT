using DataServiceLayer;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using WebServiceLayer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebServiceLayer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Kestrel to listen on all interfaces
        builder.WebHost.UseUrls("http://0.0.0.0:3000");

        // Register DbContext
        builder.Services.AddDbContext<ImdbContext>();

        // Register all data services for dependency injection
        builder.Services.AddScoped<UserHistoryDataService>();
        builder.Services.AddScoped<UserDataService>();
        builder.Services.AddScoped<TitleDataService>();
        builder.Services.AddScoped<CrewDataService>();
        builder.Services.AddScoped<BookmarkDataService>();
        builder.Services.AddScoped<UserRatingDataService>();

        // Hasing service
        builder.Services.AddSingleton<Hashing>();
        
        // Register utility services
        builder.Services.AddScoped<Hashing>();
        
        // Register Mapster for object mapping
        builder.Services.AddMapster();
        // Add controllers
        builder.Services.AddControllers();

        // JWT Authentication
        var jwtSecret = builder.Configuration.GetSection("Auth:Secret").Value
            ?? throw new InvalidOperationException("Auth:Secret is not configured");

         builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opt =>
            {
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        // Add CORS for frontend
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                    "http://localhost:80", 
                    "http://localhost:3001",
                    "http://newtlike.com",
                    "http://newtlike.com:80",
                    "http://newtlike.com:3001"
                ) // Allow frontend from localhost and external domain
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

        var app = builder.Build();

        // Note: HTTPS redirection disabled to allow both HTTP and HTTPS during development
        // app.UseHttpsRedirection();
        
        app.UseCors("AllowFrontend"); // CORS first
        app.UseAuthentication();      // Authentication second
        app.UseAuthorization();       // Authorization third
        app.MapControllers();         // Controllers last


        app.Run();
    }
}