using BuildingBlocks.Contracts.IntegrationEvents.Customers;
using Identity.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Application.Abstractions;
using Notifications.Application.Options;
using Notifications.Application.Templates.Emails;

namespace Notifications.Application.IntegrationEvents.CustomerRegistered;

public sealed class CustomerRegisteredIntegrationEventHandler
    : INotificationHandler<CustomerRegisteredIntegrationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly IActivationService _activationService;
    private readonly NotificationOptions _notificationOptions;
    private readonly ILogger<CustomerRegisteredIntegrationEventHandler> _logger;

    public CustomerRegisteredIntegrationEventHandler(
        IEmailSender emailSender,
        IActivationService activationService,
        IOptions<NotificationOptions> notificationOptions,
        ILogger<CustomerRegisteredIntegrationEventHandler> logger)
    {
        _emailSender = emailSender;
        _activationService = activationService;
        _notificationOptions = notificationOptions.Value;
        _logger = logger;
    }

    public async Task Handle(
        CustomerRegisteredIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling CustomerRegisteredIntegrationEvent for {Email}",
            notification.OwnerEmail);

        var activationToken =
            await _activationService.GenerateActivationTokenAsync(
                notification.OwnerUserId,
                cancellationToken);

        var activationUrl =
            $"{_notificationOptions.FrontendBaseUrl}/activate-account" +
            $"?userId={notification.OwnerUserId}" +
            $"&token={Uri.EscapeDataString(activationToken)}";

        // Development/testing only.
        _logger.LogInformation(
            "ACTIVATION URL: {ActivationUrl}",
            activationUrl);

        var subject = "Activate your ILS account";

        var body = CustomerRegisteredEmailTemplate.Build(
            notification.OwnerName,
            notification.OwnerEmail,
            activationUrl);

        await _emailSender.SendAsync(
            notification.OwnerEmail,
            subject,
            body,
            cancellationToken);

        _logger.LogInformation(
            "Activation email sent successfully to {Email}",
            notification.OwnerEmail);
    }
}