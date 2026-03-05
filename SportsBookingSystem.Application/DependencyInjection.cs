using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SportsBookingSystem.Application.Commands.Auth.Register;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Auth.Login;
using ErrorOr;
namespace SportsBookingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RegisterCommand, ErrorOr<int>>, RegisterHandler>();
        services.AddScoped<IQueryHandler<LoginQuery, ErrorOr<LoginResult>>, LoginQueryHandler>();
        services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();
        
        return services;
    }
}