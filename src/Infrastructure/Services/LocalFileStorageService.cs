using Microsoft.AspNetCore.Hosting;
using Portfolio.Application.Interfaces;

namespace Portfolio.Infrastructure.Services;

public sealed class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    public async Task<string> SaveProjectImageAsync(Stream fileStream, string fileName, CancellationToken cancellationToken)
    {
        var uploadsRoot = Path.Combine(environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot"), "uploads", "projects");
        Directory.CreateDirectory(uploadsRoot);

        var ext = Path.GetExtension(fileName);
        var safeName = $"{Guid.NewGuid():N}{ext}";
        var absolute = Path.Combine(uploadsRoot, safeName);

        await using var output = File.Create(absolute);
        await fileStream.CopyToAsync(output, cancellationToken);

        return $"/uploads/projects/{safeName}";
    }
}
