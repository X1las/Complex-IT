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

        // Configure Kestrel to listen on both HTTP and HTTPS
        builder.WebHost.UseUrls("http://localhost:5000", "https://localhost:5001");

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
        builder.Services.AddSingleton<Utils.Hashing>();
        
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
            options.AddPolicy("AllowFrotend", policy =>
            {
                policy.WithOrigins("https://localhost:3000", "http://localhost:3000") // Allow both HTTP and HTTPS
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