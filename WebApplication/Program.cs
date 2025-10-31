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
        
        // Configure JWT Authentication
        var secret = builder.Configuration.GetSection("Auth:Secret").Value 
            ?? throw new InvalidOperationException("Auth:Secret is not configured");
        var key = Encoding.UTF8.GetBytes(secret);
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });
        
        builder.Services.AddAuthorization();
        
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
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}