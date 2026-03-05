using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class FieldsController : BaseController
{
    private readonly IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>> _getFieldById;

    public FieldsController(IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>> getFieldById)
    {
        _getFieldById = getFieldById;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFieldById(int id, CancellationToken ct)
    {
        var result = await _getFieldById.HandleAsync(new GetFieldByIdQuery(id), ct);
        return result.Match(Ok, Problem);
    }
}