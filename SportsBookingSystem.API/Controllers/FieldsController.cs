using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Application.Commands.Fields.DeleteField;
using SportsBookingSystem.Application.Commands.Fields.UpdateField;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class FieldsController : BaseController
{
    private readonly ICommandHandler<UpdateFieldCommand,ErrorOr<Updated>> _updateField;
    private readonly ICommandHandler<DeleteFieldCommand,ErrorOr<Deleted>> _deleteField;
    
    private readonly IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>> _getFieldById;
    
    public FieldsController(ICommandHandler<UpdateFieldCommand,ErrorOr<Updated>> updateField
        ,ICommandHandler<DeleteFieldCommand,ErrorOr<Deleted>> deleteField
        ,IQueryHandler<GetFieldByIdQuery, ErrorOr<FieldDto>> getFieldById)
    {
        _updateField = updateField;
        _deleteField = deleteField;
        _getFieldById = getFieldById;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetFieldById(int id, CancellationToken ct)
    {
        var result = await _getFieldById.HandleAsync(new GetFieldByIdQuery(id), ct);
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