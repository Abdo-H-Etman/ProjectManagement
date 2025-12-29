using System.Text;
using Application.Common.Settings;
using Application.Utilities;
using Domain.Entities.Models;
using Infrastructure.Data;
using Infrastructure.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API.Extensions;

public static class ServiceExtensions
{
    public static void ConfigureAllServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureDatabase(configuration);
        services.ConfigureIdentity();
        services.ConfigureCors(configuration);
        services.ConfigureJwt(configuration);
        services.ConfigureEmailService(configuration);
        services.ConfigureApplicationServices(configuration);
        services.AddRepositories();
    }
    public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContextPool<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("SQLSERVER_CONNECTION_STRING")));

    public static void ConfigureIdentity(this IServiceCollection services)
    {
        services
            .AddIdentity<ApplicationUser, IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }

    public static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", builder =>
            {
                builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public static void ConfigureJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecrestKey = Environment.GetEnvironmentVariable("JwtSettings_SECRET_KEY") 
                ?? throw new InvalidOperationException("JWT Secret Key is not configured.");
        var jwtIssuer = Environment.GetEnvironmentVariable("JwtSettings_ISSUER") 
                ?? throw new InvalidOperationException("JWT Issuer is not configured.");
        var jwtAudience = Environment.GetEnvironmentVariable("JwtSettings_AUDIENCE") 
                ?? throw new InvalidOperationException("JWT Audience is not configured.");
        var jwtExpirationInMinutes = int.Parse(Environment.GetEnvironmentVariable("JwtSettings_EXPIRATION_MINUTES") ?? "60");
        var jwtRefreshTokenExpirationInDays = int.Parse(Environment.GetEnvironmentVariable("JwtSettings_REFRESH_TOKEN_EXPIRATION_DAYS") ?? "7");

        services.Configure<JwtSettings>(options =>
        {
            options.SecretKey = jwtSecrestKey;
            options.Issuer = jwtIssuer;
            options.Audience = jwtAudience;
            options.ExpirationInMinutes = jwtExpirationInMinutes;
            options.RefreshTokenExpirationInDays = jwtRefreshTokenExpirationInDays;
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer("Bearer", options =>
        {
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecrestKey)),
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,
                ValidateAudience = true,
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();
    }

    public static void ConfigureEmailService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailSettings>(options =>
        {
            options.SmtpServer = Environment.GetEnvironmentVariable("Email_SMTP_SERVER")
                ?? throw new InvalidOperationException("SMTP Server is not configured.");
            options.SmtpPort = int.Parse(Environment.GetEnvironmentVariable("Email_SMTP_PORT") ?? "25");
            options.SenderName = Environment.GetEnvironmentVariable("Email_SENDER_NAME")
                ?? throw new InvalidOperationException("Sender Name is not configured.");
            options.SenderEmail = Environment.GetEnvironmentVariable("Email_SENDER_EMAIL")
                ?? throw new InvalidOperationException("Sender Email is not configured.");
            options.Username = Environment.GetEnvironmentVariable("Email_USERNAME") ?? string.Empty;
            options.Password = Environment.GetEnvironmentVariable("Email_PASSWORD") ?? string.Empty;
            options.EnableSsl = bool.Parse(Environment.GetEnvironmentVariable("Email_ENABLE_SSL") ?? "true");
            options.UseDefaultCredentials = bool.Parse(Environment.GetEnvironmentVariable("Email_USE_DEFAULT_CREDENTIALS") ?? "false");        
        });
    }
    
}
