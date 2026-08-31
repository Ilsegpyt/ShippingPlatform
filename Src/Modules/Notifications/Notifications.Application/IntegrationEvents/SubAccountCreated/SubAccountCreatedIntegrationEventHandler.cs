using BuildingBlocks.Contracts.IntegrationEvents.Identity;
using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Application.Templates.Emails;

namespace Notifications.Application.IntegrationEvents.SubAccountCreated;

public sealed class SubAccountCreatedIntegrationEventHandler
    : INotificationHandler<SubAccountCreatedIntegrationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SubAccountCreatedIntegrationEventHandler> _logger;

    public SubAccountCreatedIntegrationEventHandler(
        IEmailSender emailSender,
        ILogger<SubAccountCreatedIntegrationEventHandler> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Handle(
        SubAccountCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling SubAccountCreatedIntegrationEvent for {Email}",
            notification.Email);

        var subject = "Welcome to ILS";

        var body = SubAccountCreatedEmailTemplate.Build(
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
