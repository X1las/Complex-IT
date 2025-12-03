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

        // Configure Kestrel for HTTPS
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(3000, listenOptions =>
            {
                listenOptions.UseHttps("/etc/letsencrypt/live/www.newtlike.com/fullchain.pem", "/etc/letsencrypt/live/www.newtlike.com/privkey.pem");
            });
        });

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Register DbContext
        builder.Services.AddDbContext<ImdbContext>();

        // Register all data services for dependency injection
        builder.Services.AddScoped<UserHistoryDataService>();
        builder.Services.AddScoped<UserDataService>();
        builder.Services.AddScoped<TitleDataService>();
        builder.Services.AddScoped<CrewDataService>();
        builder.Services.AddScoped<BookmarkDataService>();
        builder.Services.AddScoped<UserRatingDataService>();
        builder.Services.AddScoped<Hashing>();
        
        // Register Mapster for object mapping
        builder.Services.AddMapster();

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

        // Update CORS for HTTPS
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.WithOrigins(
                    "https://newtlike.com",        // HTTPS
                    "https://www.newtlike.com",    // HTTPS
                    "http://localhost:5173",       // Dev server
                    "http://localhost:3001"        // Dev server
                )
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
        });

        var app = builder.Build();

        // Enable CORS
        app.UseCors("AllowFrontend");

        app.UseRouting();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}