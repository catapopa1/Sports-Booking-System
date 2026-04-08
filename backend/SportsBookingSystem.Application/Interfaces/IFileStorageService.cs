using FluentValidation.Validators;

namespace SportsBookingSystem.Application.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(Stream fileStream,string fileName, CancellationToken ct = default);
}