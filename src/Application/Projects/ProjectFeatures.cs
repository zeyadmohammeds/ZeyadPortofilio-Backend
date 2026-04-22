using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Common;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;
using Portfolio.Domain.Enums;

namespace Portfolio.Application.Projects;

public sealed record ProjectDto(Guid Id, string Name, string Tagline, string Description, string Type, string[] Stack, int Year, string? Url, string GithubUrl, string? ImageUrl, DateTime CreatedAtUtc);
public sealed record GetProjectsQuery(string? Type, string? Search, int Page = 1, int PageSize = 10, string SortBy = "createdAt", bool Desc = true) : IRequest<PagedResult<ProjectDto>>;
public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto>;
public sealed record UpsertProjectCommand(Guid? Id, string Name, string Tagline, string Description, string Type, string[] Stack, int Year, string? Url, string RepoName, string MetricsJson, string? ImageUrl = null) : IRequest<ProjectDto>;
public sealed record DeleteProjectCommand(Guid Id) : IRequest;

public sealed class ProjectProfile : Profile
{
    public ProjectProfile()
    {
        CreateMap<Project, ProjectDto>()
            .ForCtorParam("Type", c => c.MapFrom(s => s.Type.ToString().ToLowerInvariant()))
            .ForCtorParam("Stack", c => c.MapFrom(s => s.Stack.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)))
            .ForCtorParam("GithubUrl", c => c.MapFrom(s => $"https://github.com/zeyadmohammeds/{s.RepoName}"));
    }
}

public sealed class ProjectHandlers(IRepository<Project> projects, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetProjectsQuery, PagedResult<ProjectDto>>,
      IRequestHandler<GetProjectByIdQuery, ProjectDto>,
      IRequestHandler<UpsertProjectCommand, ProjectDto>,
      IRequestHandler<DeleteProjectCommand>
{
    public async Task<PagedResult<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var query = projects.Query().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Type) && Enum.TryParse<ProjectType>(request.Type, true, out var parsedType))
            query = query.Where(x => x.Type == parsedType);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x => x.Name.Contains(request.Search) || x.Description.Contains(request.Search) || x.Tagline.Contains(request.Search));
        }

        query = request.SortBy.ToLowerInvariant() switch
        {
            "year" when request.Desc => query.OrderByDescending(x => x.Year),
            "year" => query.OrderBy(x => x.Year),
            _ when request.Desc => query.OrderByDescending(x => x.CreatedAtUtc),
            _ => query.OrderBy(x => x.CreatedAtUtc)
        };

        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
        return new PagedResult<ProjectDto>(mapper.Map<List<ProjectDto>>(items), total, request.Page, request.PageSize);
    }

    public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
    {
        var project = await projects.Query().AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
                      ?? throw new KeyNotFoundException("Project not found.");
        return mapper.Map<ProjectDto>(project);
    }

    public async Task<ProjectDto> Handle(UpsertProjectCommand request, CancellationToken cancellationToken)
    {
        var parsedType = Enum.Parse<ProjectType>(request.Type, true);
        Project project;
        if (request.Id.HasValue)
        {
            project = await projects.GetByIdAsync(request.Id.Value, cancellationToken) ?? throw new KeyNotFoundException("Project not found.");
            project.UpdatedAtUtc = DateTime.UtcNow;
        }
        else
        {
            project = new Project();
            await projects.AddAsync(project, cancellationToken);
        }

        project.Name = request.Name;
        project.Tagline = request.Tagline;
        project.Description = request.Description;
        project.Type = parsedType;
        project.Stack = string.Join(',', request.Stack.Distinct(StringComparer.OrdinalIgnoreCase));
        project.Year = request.Year;
        project.Url = request.Url;
        project.RepoName = request.RepoName;
        project.MetricsJson = request.MetricsJson;
        project.ImageUrl = request.ImageUrl;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<ProjectDto>(project);
    }

    public async Task<Unit> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await projects.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException("Project not found.");
        project.IsDeleted = true;
        project.UpdatedAtUtc = DateTime.UtcNow;
        projects.Update(project);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
