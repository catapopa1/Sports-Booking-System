using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Commands.Fields.DeleteField;
using SportsBookingSystem.Application.Commands.Fields.UpdateField;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Fields.GetFieldAvailability;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class FieldsController : BaseController
{
    private readonly ICommandHandler<UpdateFieldCommand,ErrorOr<Updated>> _updateField;
    private readonly ICommandHandler<DeleteFieldCommand,ErrorOr<Deleted>> _deleteField;

    private readonly IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>> _getFieldById;
    private readonly IQueryHandler<GetFieldAvailabilityQuery, ErrorOr<List<FieldOccupancySlotDto>>> _getFieldAvailability;

    public FieldsController(ICommandHandler<UpdateFieldCommand,ErrorOr<Updated>> updateField
        ,ICommandHandler<DeleteFieldCommand,ErrorOr<Deleted>> deleteField
        ,IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>> getFieldById
        ,IQueryHandler<GetFieldAvailabilityQuery, ErrorOr<List<FieldOccupancySlotDto>>> getFieldAvailability)
    {
        _updateField = updateField;
        _deleteField = deleteField;
        _getFieldById = getFieldById;
        _getFieldAvailability = getFieldAvailability;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFieldById(int id, CancellationToken ct)
    {
        var result = await _getFieldById.HandleAsync(new GetFieldByIdQuery(id), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("{id}/availability")]
    public async Task<IActionResult> GetFieldAvailability(
        int id,
        [FromQuery] DateTimeOffset from,
        [FromQuery] DateTimeOffset to,
        CancellationToken ct)
    {
        var result = await _getFieldAvailability.HandleAsync(new GetFieldAvailabilityQuery(id, from, to), ct);
        return result.Match(Ok, Problem);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateField(int id, [FromBody] UpdateFieldCommand command, CancellationToken ct)
    {
        var result = await _updateField.HandleAsync(command with { FieldId = id }, ct);
        return result.Match(_ => Ok(), Problem);
    }
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteField(int id, CancellationToken ct)
    {
        var result = await _deleteField.HandleAsync(new DeleteFieldCommand(id), ct);
        return result.Match(_ => NoContent(), Problem);
    }


}