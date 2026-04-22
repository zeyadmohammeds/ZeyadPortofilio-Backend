using Microsoft.Extensions.Logging;
using Portfolio.Application.Interfaces;

namespace Portfolio.Infrastructure.Services;

public sealed class ConsoleEmailService(ILogger<ConsoleEmailService> logger) : IEmailService
{
    public Task SendContactReceivedAsync(string toEmail, string name, string message, CancellationToken cancellationToken)
    {
        logger.LogInformation("Contact message received from {Name} <{Email}>: {Message}", name, toEmail, message);
        return Task.CompletedTask;
    }
}
