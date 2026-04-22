using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Common;
using Portfolio.Application.Education;
using Portfolio.Application.Contact;
using Portfolio.Application.Interfaces;
using Portfolio.Application.Projects;

namespace Portfolio.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Authorize(Policy = "AdminOnly")]
[Route("api/v{version:apiVersion}/admin")]
public sealed class AdminController(IMediator mediator, IFileStorageService fileStorageService) : ControllerBase
{
    [HttpPost("projects")]
    public async Task<IActionResult> UpsertProject([FromBody] UpsertProjectCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(new ApiEnvelope<ProjectDto>(true, 200, "Project saved.", result));
    }

    [HttpDelete("projects/{id:guid}")]
    public async Task<IActionResult> DeleteProject(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProjectCommand(id), cancellationToken);
        return Ok(new ApiEnvelope<object>(true, 200, "Project deleted."));
    }

    [HttpPost("projects/{id:guid}/image")]
    [RequestSizeLimit(5 * 1024 * 1024)]
    public async Task<IActionResult> UploadProjectImage(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new ApiEnvelope<object>(false, 400, "File is required."));
        }

        if (!file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiEnvelope<object>(false, 400, "Only image files are allowed."));
        }

        await using var stream = file.OpenReadStream();
        var imageUrl = await fileStorageService.SaveProjectImageAsync(stream, file.FileName, cancellationToken);

        var project = await mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        var updated = await mediator.Send(new UpsertProjectCommand(project.Id, project.Name, project.Tagline, project.Description, project.Type, project.Stack, project.Year, project.Url, project.GithubUrl.Split('/').Last(), "{}", imageUrl), cancellationToken);
        return Ok(new ApiEnvelope<object>(true, 200, "Image uploaded.", new { imageUrl, project = updated }));
    }

    [HttpPost("education")]
    public async Task<IActionResult> UpsertEducation([FromBody] UpsertEducationCommand command, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(command, cancellationToken);
        return Ok(new ApiEnvelope<EducationDto>(true, 200, "Education saved.", result));
    }

    [HttpDelete("education/{id:guid}")]
    public async Task<IActionResult> DeleteEducation(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteEducationCommand(id), cancellationToken);
        return Ok(new ApiEnvelope<object>(true, 200, "Education deleted."));
    }

    [HttpGet("stats")]
    public async Task<IActionResult> Stats(CancellationToken cancellationToken)
    {
        var projects = await mediator.Send(new GetProjectsQuery(null, null, 1, 1), cancellationToken);
        var education = await mediator.Send(new GetEducationQuery(), cancellationToken);
        var contacts = await mediator.Send(new GetContactsQuery(), cancellationToken);
        return Ok(new ApiEnvelope<object>(true, 200, "Stats loaded", new { 
            projects = projects.TotalCount, 
            education = education.Count,
            contacts = contacts.Count
        }));
    }
}
