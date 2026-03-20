namespace SportsBookingSystem.Application.Common;

public record PaginationParams(int Page = 1, int PageSize = 20)
{
    public int Skip => (Page -1) * PageSize;
}
