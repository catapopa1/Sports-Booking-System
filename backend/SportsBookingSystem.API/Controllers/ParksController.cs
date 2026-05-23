using ErrorOr;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.API.Requests;
using SportsBookingSystem.Application.Commands.Fields.CreateField;
using SportsBookingSystem.Application.Commands.Parks.CreatePark;
using SportsBookingSystem.Application.Commands.Parks.DeletePark;
using SportsBookingSystem.Application.Commands.Parks.Photos.DeleteParkPhoto;
using SportsBookingSystem.Application.Commands.Parks.Photos.SetMainParkPhoto;
using SportsBookingSystem.Application.Commands.Parks.Photos.UploadParkPhoto;
using SportsBookingSystem.Application.Commands.Parks.UpdatePark;
using SportsBookingSystem.Application.Common;
using SportsBookingSystem.Application.Queries.Dtos;
using SportsBookingSystem.Application.Queries.Fields.GetFieldById;
using SportsBookingSystem.Application.Queries.Parks.GetAllParks;
using SportsBookingSystem.Application.Queries.Parks.GetFieldsByPark;
using SportsBookingSystem.Application.Queries.Parks.GetParkById;
using SportsBookingSystem.Application.Queries.Parks.GetParkStats;

namespace SportsBookingSystem.API.Controllers;

[Route("api/[controller]")]
[Authorize]
public class ParksController : BaseController
{
    private readonly ICommandHandler<CreateParkCommand, ErrorOr<int>> _createPark;
    private readonly ICommandHandler<CreateFieldCommand, ErrorOr<int>> _createField;
    private readonly ICommandHandler<UpdateParkCommand,ErrorOr<Updated>> _updatePark;
    private readonly ICommandHandler<DeleteParkCommand, ErrorOr<Deleted>> _deletePark;
    private readonly ICommandHandler<UploadParkPhotoCommand, ErrorOr<ParkPhotoDto>> _uploadParkPhoto;
    private readonly ICommandHandler<DeleteParkPhotoCommand, ErrorOr<Deleted>> _deleteParkPhoto;
    private readonly ICommandHandler<SetMainParkPhotoCommand, ErrorOr<Updated>> _setMainParkPhoto;

    private readonly IQueryHandler<GetAllParksQuery, ErrorOr<List<ParkSummaryDto>>> _getAllParks;
    private readonly IQueryHandler<GetParkByIdQuery, ErrorOr<ParkDto>> _getParkById;
    private readonly IQueryHandler<GetFieldsByParkQuery, ErrorOr<List<FieldDto>>> _getFieldsByPark;
    private readonly IQueryHandler<GetParkStatsQuery, ErrorOr<ParkStatsDto>> _getParkStatsHandler;

    public ParksController(
        ICommandHandler<CreateParkCommand, ErrorOr<int>> createPark,
        ICommandHandler<CreateFieldCommand, ErrorOr<int>> createField,
        ICommandHandler<UpdateParkCommand, ErrorOr<Updated>> updatePark,
        ICommandHandler<DeleteParkCommand, ErrorOr<Deleted>> deletePark,
        ICommandHandler<UploadParkPhotoCommand, ErrorOr<ParkPhotoDto>> uploadParkPhoto,
        ICommandHandler<DeleteParkPhotoCommand, ErrorOr<Deleted>> deleteParkPhoto,
        ICommandHandler<SetMainParkPhotoCommand, ErrorOr<Updated>> setMainParkPhoto,
        IQueryHandler<GetAllParksQuery, ErrorOr<List<ParkSummaryDto>>> getAllParks,
        IQueryHandler<GetParkByIdQuery, ErrorOr<ParkDto>> getParkById,
        IQueryHandler<GetFieldsByParkQuery, ErrorOr<List<FieldDto>>> getFieldsByPark,
        IQueryHandler<GetParkStatsQuery, ErrorOr<ParkStatsDto>> getParkStatsHandler)
    {
        _createPark = createPark;
        _createField = createField;
        _updatePark = updatePark;
        _deletePark = deletePark;
        _uploadParkPhoto = uploadParkPhoto;
        _deleteParkPhoto = deleteParkPhoto;
        _setMainParkPhoto = setMainParkPhoto;
        _getAllParks = getAllParks;
        _getParkById = getParkById;
        _getFieldsByPark = getFieldsByPark;
        _getParkStatsHandler = getParkStatsHandler;
    }
    

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreatePark(CreateParkCommand command,CancellationToken ct)
    {
        var result = await _createPark.HandleAsync(command,ct);
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
    
    [HttpGet("{id}/stats")]
    [Authorize(Roles = "ParkManager,Admin")]
    public async Task<IActionResult> GetParkStats(int id, CancellationToken ct)
    {
        var result = await _getParkStatsHandler.HandleAsync(new GetParkStatsQuery(id), ct);
        return result.Match(Ok, Problem);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdatePark(int id, [FromBody] UpdateParkCommand command, CancellationToken ct)
    {
        var result = await _updatePark.HandleAsync(command with { ParkId = id }, ct);
        return result.Match(_ => Ok(), Problem);
    }

    
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeletePark(int id, CancellationToken ct)
    {
        var result = await _deletePark.HandleAsync(new DeleteParkCommand(id), ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPost("{parkId}/photos")]
    [Authorize]
    public async Task<IActionResult> UploadPhoto(int parkId, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
            return Problem(new List<Error> { Error.Validation("File.Empty", "A photo file is required.") });

        await using var stream = file.OpenReadStream();
        var result = await _uploadParkPhoto.HandleAsync(
            new UploadParkPhotoCommand(parkId, stream, file.FileName), ct);
        return result.Match(Ok, Problem);
    }

    [HttpDelete("{parkId}/photos/{photoId}")]
    [Authorize]
    public async Task<IActionResult> DeletePhoto(int parkId, int photoId, CancellationToken ct)
    {
        var result = await _deleteParkPhoto.HandleAsync(new DeleteParkPhotoCommand(parkId, photoId), ct);
        return result.Match(_ => NoContent(), Problem);
    }

    [HttpPut("{parkId}/photos/{photoId}/main")]
    [Authorize]
    public async Task<IActionResult> SetMainPhoto(int parkId, int photoId, CancellationToken ct)
    {
        var result = await _setMainParkPhoto.HandleAsync(new SetMainParkPhotoCommand(parkId, photoId), ct);
        return result.Match(_ => Ok(), Problem);
    }
}