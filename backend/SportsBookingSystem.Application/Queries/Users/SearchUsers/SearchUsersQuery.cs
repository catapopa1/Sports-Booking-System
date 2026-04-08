using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Users.SearchUsers;

public record SearchUsersQuery(string SearchTerm, int Page = 1, int PageSize = 20) : IQuery<ErrorOr<PagedResult<UserSearchResultDto>>>;
