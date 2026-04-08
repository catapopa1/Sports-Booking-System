using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsBookingSystem.Application.Interfaces;
using SportsBookingSystem.Infrastructure.Jobs;
using SportsBookingSystem.Infrastructure.Persistence;
using SportsBookingSystem.Infrastructure.Services;

namespace SportsBookingSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<AppDbContext>());

        services.AddScoped<ITokenService,TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddHangfire(c => c                                                                                                                   
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection")));
        services.AddHangfireServer();
        
        services.AddScoped<ProcessOutboxMessagesJob>();
        
        services.AddScoped<TimeoutStaleBookingsJob>();

        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<INotificationPusher, SignalRNotificationPusher>();
        services.AddSignalR();

        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>("database");

        return services;
    }
}