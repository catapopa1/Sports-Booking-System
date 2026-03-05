using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.API.Requests;
using SportsBookingSystem.Application.Commands.Fields.CreateField;
using SportsBookingSystem.Application.Commands.Parks.CreatePark;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
using SportsBookingSystem.Application.Queries.Parks.GetAllParks;
using SportsBookingSystem.Application.Queries.Parks.GetFieldsByPark;
using SportsBookingSystem.Application.Queries.Parks.GetParkById;
namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ParksController : BaseController
{
    private readonly ICommandHandler<CreateParkCommand, ErrorOr<int>> _createPark;
    private readonly ICommandHandler<CreateFieldCommand, ErrorOr<int>> _createField;

    private readonly IQueryHandler<GetAllParksQuery, ErrorOr<List<ParkSummaryDto>>> _getAllParks;
    private readonly IQueryHandler<GetParkByIdQuery, ErrorOr<ParkDto>> _getParkById;
    private readonly IQueryHandler<GetFieldsByParkQuery, ErrorOr<List<FieldDto>>> _getFieldsByPark;

    public ParksController(
        ICommandHandler<CreateParkCommand, ErrorOr<int>> createPark,
        ICommandHandler<CreateFieldCommand, ErrorOr<int>> createField,
        IQueryHandler<GetAllParksQuery, ErrorOr<List<ParkSummaryDto>>> getAllParks,
        IQueryHandler<GetParkByIdQuery, ErrorOr<ParkDto>> getParkById,
        IQueryHandler<GetFieldsByParkQuery, ErrorOr<List<FieldDto>>> getFieldsByPark
        )
    {
        _createPark = createPark;
        _createField = createField;
        _getAllParks = getAllParks;
        _getParkById = getParkById;
        _getFieldsByPark = getFieldsByPark;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePark(CreateParkCommand command,CancellationToken ct)
    {
        var result = await _createPark.HandleAsync(command, ct);

        return result.Match(
            id => CreatedAtAction(nameof(GetParkById), new { id }, new { id }),
            Problem);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllParks(CancellationToken ct)
    {
        var result = await _getAllParks.HandleAsync(new GetAllParksQuery(), ct);
        return result.Match(Ok, Problem);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetParkById(int id, CancellationToken ct)
    {
        var result = await _getParkById.HandleAsync(new GetParkByIdQuery(id), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPost("{id}/fields")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateField(int id, CreateFieldRequest request, CancellationToken ct)
    {
        var command = new CreateFieldCommand(id, request.Name, request.SportType, request.BaseHourlyPrice);
        var result = await _createField.HandleAsync(command, ct);

        return result.Match(fieldId => CreatedAtAction(nameof(FieldsController.GetFieldById), "Fields",
            new { id = fieldId }, new { id = fieldId }),
            Problem);
        
    }

    [HttpGet("{id}/fields")]
    public async Task<IActionResult> GetFieldsByPark(int id, CancellationToken ct)
    {
        var result = await _getFieldsByPark.HandleAsync(new GetFieldsByParkQuery(id), ct);
        return result.Match(Ok, Problem);
    }
}