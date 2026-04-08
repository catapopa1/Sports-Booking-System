using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SportsBookingSystem.Application.Commands.Auth.Register;
using SportsBookingSystem.Application.Commands.Bookings.ApproveBooking;
using SportsBookingSystem.Application.Commands.Bookings.CancelBooking;
using SportsBookingSystem.Application.Commands.Bookings.CreateBooking;
using SportsBookingSystem.Application.Commands.Bookings.RespondToInvite;
using SportsBookingSystem.Application.Commands.Fields.CreateField;
using SportsBookingSystem.Application.Commands.Fields.DeleteField;
using SportsBookingSystem.Application.Commands.Fields.UpdateField;
using SportsBookingSystem.Application.Commands.Friendships.RemoveFriend;
using SportsBookingSystem.Application.Commands.Friendships.RespondToFriendRequestCommand;
using SportsBookingSystem.Application.Commands.Friendships.SendFriendRequest;
using SportsBookingSystem.Application.Commands.Notifications.MarkAllNotificationsRead;
using SportsBookingSystem.Application.Commands.Notifications.MarkNotificationRead;
using SportsBookingSystem.Application.Commands.Parks;
using SportsBookingSystem.Application.Commands.Parks.CreatePark;
using SportsBookingSystem.Application.Commands.Parks.DeletePark;
using SportsBookingSystem.Application.Commands.Parks.UpdatePark;
using SportsBookingSystem.Application.Commands.Users.ChangePassword;
using SportsBookingSystem.Application.Commands.Users.UpdateProfile;
using SportsBookingSystem.Application.Commands.Users.UpdateUserRole;
using SportsBookingSystem.Application.Commands.Users.UploadAvatar;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Auth.Login;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
using SportsBookingSystem.Application.Queries.Friendships.GetMyFriends;
using SportsBookingSystem.Application.Queries.Friendships.GetPendingRequests;
using SportsBookingSystem.Application.Queries.Friendships.GetSentRequests;
using SportsBookingSystem.Application.Queries.Bookings.GetBookingById;
using SportsBookingSystem.Application.Queries.Bookings.GetMyBookings;
using SportsBookingSystem.Application.Queries.Bookings.GetMyInvites;
using SportsBookingSystem.Application.Queries.Bookings.GetPendingApprovals;
using SportsBookingSystem.Application.Queries.Notifications.GetMyNotifications;
using SportsBookingSystem.Application.Queries.Parks.GetAllParks;
using SportsBookingSystem.Application.Queries.Parks.GetFieldsByPark;
using SportsBookingSystem.Application.Queries.Parks.GetParkById;
using SportsBookingSystem.Application.Queries.Parks.GetParkStats;
using SportsBookingSystem.Application.Queries.Users.GetAllUsers;
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
        services.AddScoped<ICommandHandler<UpdateParkCommand, ErrorOr<Updated>>, UpdateParkHandler>();
        services.AddScoped<ICommandHandler<DeleteParkCommand, ErrorOr<Deleted>>, DeleteParkHandler>();

        // Fields
        services.AddScoped<ICommandHandler<CreateFieldCommand, ErrorOr<int>>, CreateFieldHandler>();
        services.AddScoped<IQueryHandler<GetFieldsByParkQuery, ErrorOr<List<FieldDto>>>, GetFieldsByParkHandler>();
        services.AddScoped<IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>>, GetFieldByIdHandler>();
        services.AddScoped<IValidator<CreateFieldCommand>, CreateFieldCommandValidator>();
        services.AddScoped<ICommandHandler<UpdateFieldCommand, ErrorOr<Updated>>, UpdateFieldHandler>();
        services.AddScoped<ICommandHandler<DeleteFieldCommand, ErrorOr<Deleted>>, DeleteFieldHandler>();


        // Friendships
        services.AddScoped<ICommandHandler<SendFriendRequestCommand, ErrorOr<int>>, SendFriendRequestHandler>();
        services.AddScoped<ICommandHandler<RespondtoFriendRequestCommand, ErrorOr<Updated>>, RespondToFriendRequestHandler>();
        services.AddScoped<ICommandHandler<RemoveFriendCommand, ErrorOr<Deleted>>, RemoveFriendHandler>();
        services.AddScoped<IValidator<SendFriendRequestCommand>, SendFriendRequestCommandValidator>();
        services.AddScoped<IQueryHandler<GetMyFriendsQuery, ErrorOr<List<FriendDto>>>, GetMyFriendsHandler>();
        services.AddScoped<IQueryHandler<GetPendingRequestsQuery, ErrorOr<List<FriendRequestDto>>>, GetPendingRequestsHandler>();
        services.AddScoped<IQueryHandler<GetSentRequestsQuery, ErrorOr<List<FriendRequestDto>>>, GetSentRequestsHandler>();

        // Users
        services.AddScoped<IQueryHandler<SearchUsersQuery, ErrorOr<PagedResult<UserSearchResultDto>>>, SearchUsersHandler>();
        services.AddScoped<IQueryHandler<GetUserProfileQuery, ErrorOr<UserProfileDto>>, GetUserProfileHandler>();
        services.AddScoped<ICommandHandler<UpdateProfileCommand, ErrorOr<Updated>>, UpdateProfileHandler>();
        services.AddScoped<ICommandHandler<UploadAvatarCommand, ErrorOr<string>>, UploadAvatarHandler>();
        services.AddScoped<ICommandHandler<ChangePasswordCommand, ErrorOr<Updated>>, ChangePasswordHandler>();

        // Bookings 
        services.AddScoped<ICommandHandler<CreateBookingCommand, ErrorOr<int>>, CreateBookingHandler>();
        services.AddScoped<ICommandHandler<RespondToInviteCommand, ErrorOr<Updated>>, RespondToInviteHandler>();
        services.AddScoped<ICommandHandler<ApproveBookingCommand, ErrorOr<Updated>>, ApproveBookingHandler>();
        services.AddScoped<ICommandHandler<CancelBookingCommand, ErrorOr<Updated>>, CancelBookingHandler>();
        services.AddScoped<IValidator<CreateBookingCommand>, CreateBookingValidator>();
        services.AddScoped<IQueryHandler<GetBookingByIdQuery, ErrorOr<BookingDto>>, GetBookingByIdHandler>();
        services.AddScoped<IQueryHandler<GetMyBookingsQuery, ErrorOr<PagedResult<BookingSummaryDto>>>, GetMyBookingsHandler>();
        services.AddScoped<IQueryHandler<GetMyInvitesQuery, ErrorOr<List<InviteNotificationDto>>>, GetMyInvitesHandler>();
        services.AddScoped<IQueryHandler<GetPendingApprovalsQuery, ErrorOr<List<BookingSummaryDto>>>, GetPendingApprovalsHandler>();
        services.AddScoped<IQueryHandler<GetParkStatsQuery, ErrorOr<ParkStatsDto>>, GetParkStatsQueryHandler>();
        
        // Notifications
        services.AddScoped<IQueryHandler<GetMyNotificationsQuery, ErrorOr<PagedResult<NotificationDto>>>, GetMyNotificationsHandler>();
        services.AddScoped<ICommandHandler<MarkNotificationReadCommand, ErrorOr<Updated>>, MarkNotificationReadHandler>();
        services.AddScoped<ICommandHandler<MarkAllNotificationsReadCommand, ErrorOr<Updated>>, MarkAllNotificationsReadHandler>();

        //Admin
        services.AddScoped<IQueryHandler<GetAllUsersQuery, ErrorOr<PagedResult<AdminUserDto>>>, GetAllUsersHandler>();
        services.AddScoped<ICommandHandler<UpdateUserRoleCommand, ErrorOr<Updated>>, UpdateUserRoleHandler>();
        
        return services;
    }
}
