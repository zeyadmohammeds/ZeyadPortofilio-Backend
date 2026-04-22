using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Education;

public sealed record EducationDto(Guid Id, string School, string Degree, string? Focus, DateOnly StartDate, DateOnly EndDate, string? Notes);
public sealed record GetEducationQuery : IRequest<IReadOnlyCollection<EducationDto>>;
public sealed record UpsertEducationCommand(Guid? Id, string School, string Degree, string? Focus, DateOnly StartDate, DateOnly EndDate, string? Notes) : IRequest<EducationDto>;
public sealed record DeleteEducationCommand(Guid Id) : IRequest;

public sealed class EducationProfile : Profile
{
    public EducationProfile() => CreateMap<Portfolio.Domain.Entities.Education, EducationDto>();
}

public sealed class EducationHandlers(IRepository<Portfolio.Domain.Entities.Education> education, IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<GetEducationQuery, IReadOnlyCollection<EducationDto>>,
      IRequestHandler<UpsertEducationCommand, EducationDto>,
      IRequestHandler<DeleteEducationCommand>
{
    public async Task<IReadOnlyCollection<EducationDto>> Handle(GetEducationQuery request, CancellationToken cancellationToken)
    {
        var items = await education.Query().AsNoTracking().OrderByDescending(x => x.EndDate).ToListAsync(cancellationToken);
        return mapper.Map<IReadOnlyCollection<EducationDto>>(items);
    }

    public async Task<EducationDto> Handle(UpsertEducationCommand request, CancellationToken cancellationToken)
    {
        Portfolio.Domain.Entities.Education entity;
        if (request.Id.HasValue)
        {
            entity = await education.GetByIdAsync(request.Id.Value, cancellationToken) ?? throw new KeyNotFoundException("Education not found.");
            entity.UpdatedAtUtc = DateTime.UtcNow;
        }
        else
        {
            entity = new Portfolio.Domain.Entities.Education();
            await education.AddAsync(entity, cancellationToken);
        }

        entity.School = request.School;
        entity.Degree = request.Degree;
        entity.Focus = request.Focus;
        entity.StartDate = request.StartDate;
        entity.EndDate = request.EndDate;
        entity.Notes = request.Notes;

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return mapper.Map<EducationDto>(entity);
    }

    public async Task<Unit> Handle(DeleteEducationCommand request, CancellationToken cancellationToken)
    {
        var entity = await education.GetByIdAsync(request.Id, cancellationToken) ?? throw new KeyNotFoundException("Education not found.");
        entity.IsDeleted = true;
        entity.UpdatedAtUtc = DateTime.UtcNow;
        education.Update(entity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
