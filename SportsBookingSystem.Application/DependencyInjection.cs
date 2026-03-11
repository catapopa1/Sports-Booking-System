using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SportsBookingSystem.Application.Commands.Auth.Register;
using SportsBookingSystem.Application.Commands.Fields.CreateField;
using SportsBookingSystem.Application.Commands.Friendships.RemoveFriend;
using SportsBookingSystem.Application.Commands.Friendships.RespondToFriendRequestCommand;
using SportsBookingSystem.Application.Commands.Friendships.SendFriendRequest;
using SportsBookingSystem.Application.Commands.Parks;
using SportsBookingSystem.Application.Commands.Parks.CreatePark;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Auth.Login;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
using SportsBookingSystem.Application.Queries.Friendships.GetMyFriends;
using SportsBookingSystem.Application.Queries.Friendships.GetPendingRequests;
using SportsBookingSystem.Application.Queries.Friendships.GetSentRequests;
using SportsBookingSystem.Application.Queries.Parks.GetAllParks;
using SportsBookingSystem.Application.Queries.Parks.GetFieldsByPark;
using SportsBookingSystem.Application.Queries.Parks.GetParkById;
using SportsBookingSystem.Application.Queries.Users.GetUserProfile;
using SportsBookingSystem.Application.Queries.Users.SearchUsers;

namespace SportsBookingSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Auth
        services.AddScoped<ICommandHandler<RegisterCommand, ErrorOr<int>>, RegisterHandler>();
        services.AddScoped<IQueryHandler<LoginQuery, ErrorOr<LoginResult>>, LoginQueryHandler>();
        services.AddScoped<IValidator<RegisterCommand>, RegisterCommandValidator>();

        // Parks
        services.AddScoped<ICommandHandler<CreateParkCommand, ErrorOr<int>>, CreateParkHandler>();
        services.AddScoped<IQueryHandler<GetAllParksQuery, ErrorOr<List<ParkSummaryDto>>>, GetAllParksHandler>();
        services.AddScoped<IQueryHandler<GetParkByIdQuery, ErrorOr<ParkDto>>, GetParkByIdHandler>();
        services.AddScoped<IValidator<CreateParkCommand>, CreateParkCommandValidator>();

        // Fields
        services.AddScoped<ICommandHandler<CreateFieldCommand, ErrorOr<int>>, CreateFieldHandler>();
        services.AddScoped<IQueryHandler<GetFieldsByParkQuery, ErrorOr<List<FieldDto>>>, GetFieldsByParkHandler>();
        services.AddScoped<IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>>, GetFieldByIdHandler>();
        services.AddScoped<IValidator<CreateFieldCommand>, CreateFieldCommandValidator>();

        // Friendships
        services.AddScoped<ICommandHandler<SendFriendRequestCommand, ErrorOr<int>>, SendFriendRequestHandler>();
        services.AddScoped<ICommandHandler<RespondtoFriendRequestCommand, ErrorOr<Updated>>, RespondToFriendRequestHandler>();
        services.AddScoped<ICommandHandler<RemoveFriendCommand, ErrorOr<Deleted>>, RemoveFriendHandler>();
        services.AddScoped<IValidator<SendFriendRequestCommand>, SendFriendRequestCommandValidator>();
        services.AddScoped<IQueryHandler<GetMyFriendsQuery, ErrorOr<List<FriendDto>>>, GetMyFriendsHandler>();
        services.AddScoped<IQueryHandler<GetPendingRequestsQuery, ErrorOr<List<FriendRequestDto>>>, GetPendingRequestsHandler>();
        services.AddScoped<IQueryHandler<GetSentRequestsQuery, ErrorOr<List<FriendRequestDto>>>, GetSentRequestsHandler>();

        // Users
        services.AddScoped<IQueryHandler<SearchUsersQuery, ErrorOr<List<UserSearchResultDto>>>, SearchUsersHandler>();
        services.AddScoped<IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>>, GetUserProfileHandler>();

        return services;
    }
}
