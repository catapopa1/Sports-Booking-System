using Microsoft.AspNetCore.Hosting;
using SportsBookingSystem.Application.Interfaces;
namespace SportsBookingSystem.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _uploadsPath;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        _uploadsPath = Path.Combine(env.WebRootPath, "uploads");
        Directory.CreateDirectory(_uploadsPath);
    }

    public async Task<string> SaveAsync(Stream fileStream, string fileName, CancellationToken ct = default)
    {
        var extension = Path.GetExtension(fileName);
        var storedFilename = $"{Guid.NewGuid()}{extension}";
        var fullPath = Path.Combine(_uploadsPath, storedFilename);
        
        await using var stream = File.Create(fullPath);
        await fileStream.CopyToAsync(stream, ct);
        
        return $"/uploads/{storedFilename}";
    }
}