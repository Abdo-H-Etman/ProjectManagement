using Application.Common.Models.Interfaces;
using Application.Interfaces.Auth;
using Application.Interfaces.Logging;
using Application.Interfaces.Mailing;
using Application.Services;
using Application.Services.Auth;
using Application.Services.Logging;
using Application.Services.Mailing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Utilities;

public static class ServiceExtension
{
    public static void ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddSingleton<ILoggerManager, LoggerManager>();
    }
}
