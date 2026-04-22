using MediatR;
using Microsoft.EntityFrameworkCore;
using Portfolio.Application.Interfaces;
using Portfolio.Domain.Entities;

namespace Portfolio.Application.Contact;

public sealed record SubmitContactCommand(string Name, string Email, string Message) : IRequest<Guid>;
public sealed record GetContactsQuery() : IRequest<IReadOnlyCollection<ContactDto>>;
public sealed record DeleteContactCommand(Guid Id) : IRequest<Unit>;
public sealed record UpdateContactCommand(Guid Id, bool Processed, string? Message = null) : IRequest<Unit>;
public sealed record ContactDto(Guid Id, string Name, string Email, string Message, DateTime CreatedAt, bool Processed);

public sealed class ContactHandlers(IRepository<ContactSubmission> contacts, IUnitOfWork unitOfWork, IEmailService emailService)
    : IRequestHandler<SubmitContactCommand, Guid>,
      IRequestHandler<GetContactsQuery, IReadOnlyCollection<ContactDto>>,
      IRequestHandler<DeleteContactCommand, Unit>,
      IRequestHandler<UpdateContactCommand, Unit>
{
    public async Task<Guid> Handle(SubmitContactCommand request, CancellationToken cancellationToken)
    {
        var submission = new ContactSubmission
        {
            Name = request.Name,
            Email = request.Email,
            Message = request.Message
        };

        await contacts.AddAsync(submission, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await emailService.SendContactReceivedAsync(request.Email, request.Name, request.Message, cancellationToken);
        return submission.Id;
    }

    public async Task<IReadOnlyCollection<ContactDto>> Handle(GetContactsQuery request, CancellationToken cancellationToken)
    {
        var list = await contacts.Query()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ContactDto(x.Id, x.Name, x.Email, x.Message, x.CreatedAtUtc, x.Processed))
            .ToListAsync(cancellationToken);
            
        return list;
    }

    public async Task<Unit> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
    {
        var submission = await contacts.GetByIdAsync(request.Id, cancellationToken);
        if (submission != null)
        {
            contacts.Remove(submission);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }

    public async Task<Unit> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
    {
        var submission = await contacts.GetByIdAsync(request.Id, cancellationToken);
        if (submission != null)
        {
            submission.Processed = request.Processed;
            if (request.Message != null)
            {
                submission.Message = request.Message;
            }
            contacts.Update(submission);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}
