using ErrorOr;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;

namespace SportsBookingSystem.Application.Queries.Users.SearchUsers;

public record SearchUsersQuery(string SearchTerm) : IQuery<ErrorOr<List<UserSearchResultDto>>>;
