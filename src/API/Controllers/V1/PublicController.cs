using MediatR;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Common;
using Portfolio.Application.Education;
using Portfolio.Application.Projects;

namespace Portfolio.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
public sealed class PublicController(IMediator mediator) : ControllerBase
{
    [HttpGet("about")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetAbout()
    {
        var profile = new
        {
            name = "Zeyad Mohammed",
            title = "Junior Full-Stack Engineer",
            location = "Cairo, Egypt",
            pitch = "I design and ship resilient web platforms — from Postgres schemas to pixel-tight UI.",
            bio = "I focus on architecture, reliability, and developer experience to keep product velocity high.",
            availability = "Open to junior IC roles · Egypt/Remote"
        };

        return Ok(new ApiEnvelope<object>(true, 200, "Profile loaded", profile));
    }

    [HttpGet("projects")]
    [ProducesResponseType(typeof(ApiEnvelope<PagedResult<ProjectDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects([FromQuery] string? type, [FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "createdAt", [FromQuery] bool desc = true, CancellationToken cancellationToken = default)
    {
        var result = await mediator.Send(new GetProjectsQuery(type, search, page, pageSize, sortBy, desc), cancellationToken);
        return Ok(new ApiEnvelope<PagedResult<ProjectDto>>(true, 200, "Projects loaded", result));
    }

    [HttpGet("projects/{id:guid}")]
    [ProducesResponseType(typeof(ApiEnvelope<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiEnvelope<ProjectDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProjectById(Guid id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProjectByIdQuery(id), cancellationToken);
        return Ok(new ApiEnvelope<ProjectDto>(true, 200, "Project loaded", result));
    }

    [HttpGet("education")]
    public async Task<IActionResult> GetEducation(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetEducationQuery(), cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyCollection<EducationDto>>(true, 200, "Education loaded", result));
    }
}
