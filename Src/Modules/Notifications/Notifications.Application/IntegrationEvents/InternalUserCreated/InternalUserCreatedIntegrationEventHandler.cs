using BuildingBlocks.Contracts.IntegrationEvents.Identity;
using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Application.Templates.Emails;

namespace Notifications.Application.IntegrationEvents.InternalUserCreated;

public sealed class InternalUserCreatedIntegrationEventHandler
    : INotificationHandler<InternalUserCreatedIntegrationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<InternalUserCreatedIntegrationEventHandler> _logger;

    public InternalUserCreatedIntegrationEventHandler(
        IEmailSender emailSender,
        ILogger<InternalUserCreatedIntegrationEventHandler> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Handle(
        InternalUserCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling InternalUserCreatedIntegrationEvent for {Email}",
            notification.Email);

        var subject = "Welcome to ILS";

        var body = InternalUserCreatedEmailTemplate.Build(
            notification.Name,
            notification.Email);

        await _emailSender.SendAsync(
            notification.Email,
            subject,
            body,
            cancellationToken);

        _logger.LogInformation(
            "Email sent successfully to {Email}",
            notification.Email);
    }
}
