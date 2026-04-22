using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portfolio.Application.Common;
using Portfolio.Application.Contact;

namespace Portfolio.API.Controllers.V1;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/contact")]
public sealed class ContactController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Submit([FromBody] SubmitContactCommand command, CancellationToken cancellationToken)
    {
        var id = await mediator.Send(command, cancellationToken);
        return Ok(new ApiEnvelope<object>(true, 200, "Contact request received.", new { id }));
    }

    [HttpGet]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetContactsQuery(), cancellationToken);
        return Ok(new ApiEnvelope<IReadOnlyCollection<ContactDto>>(true, 200, "Contacts loaded", result));
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateContactCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id) return BadRequest();
        await mediator.Send(command, cancellationToken);
        return Ok(new ApiEnvelope<object>(true, 200, "Contact updated."));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteContactCommand(id), cancellationToken);
        return Ok(new ApiEnvelope<object>(true, 200, "Contact deleted."));
    }
}
