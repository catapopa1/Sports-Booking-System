using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using ErrorOr;
namespace SportsBookingSystem.Application.Queries.Users.GetAllUsers;

public record GetAllUsersQuery(string? SearchTerm = null, int Page = 1, int PageSize = 20)
    : IQuery<ErrorOr<PagedResult<AdminUserDto>>>;