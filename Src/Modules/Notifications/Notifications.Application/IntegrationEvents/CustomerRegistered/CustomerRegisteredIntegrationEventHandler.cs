using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using MediatR;
using Microsoft.Extensions.Logging;
using Notifications.Application.Abstractions;
using Notifications.Application.Templates.Emails;

namespace Notifications.Application.IntegrationEvents.CustomerRegistered;

public sealed class CustomerRegisteredIntegrationEventHandler
    : INotificationHandler<CustomerRegisteredIntegrationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<CustomerRegisteredIntegrationEventHandler> _logger;

    public CustomerRegisteredIntegrationEventHandler(
        IEmailSender emailSender,
        ILogger<CustomerRegisteredIntegrationEventHandler> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Handle(
        CustomerRegisteredIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling CustomerRegisteredIntegrationEvent for {Email}",
            notification.OwnerEmail);

        var subject = "Welcome to ILS";

        var body = CustomerRegisteredEmailTemplate.Build(
            notification.OwnerName,
            notification.OwnerEmail);

        await _emailSender.SendAsync(
            notification.OwnerEmail,
            subject,
            body,
            cancellationToken);

        _logger.LogInformation(
            "Email sent successfully to {Email}",
            notification.OwnerEmail);
    }
}
