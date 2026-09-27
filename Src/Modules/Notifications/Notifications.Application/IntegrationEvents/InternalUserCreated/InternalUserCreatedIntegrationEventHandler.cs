using BuildingBlocks.Contracts.IntegrationEvents.Identity;
using Identity.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Application.Abstractions;
using Notifications.Application.Options;
using Notifications.Application.Templates.Emails;

namespace Notifications.Application.IntegrationEvents.InternalUserCreated;

public sealed class InternalUserCreatedIntegrationEventHandler
    : INotificationHandler<InternalUserCreatedIntegrationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly IActivationService _activationService;
    private readonly NotificationOptions _notificationOptions;
    private readonly ILogger<InternalUserCreatedIntegrationEventHandler> _logger;

    public InternalUserCreatedIntegrationEventHandler(
        IEmailSender emailSender,
        IActivationService activationService,
        IOptions<NotificationOptions> notificationOptions,
        ILogger<InternalUserCreatedIntegrationEventHandler> logger)
    {
        _emailSender = emailSender;
        _activationService = activationService;
        _notificationOptions = notificationOptions.Value;
        _logger = logger;
    }

    public async Task Handle(
        InternalUserCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling InternalUserCreatedIntegrationEvent for {Email}",
            notification.Email);

        var activationToken =
            await _activationService.GenerateActivationTokenAsync(
                notification.UserId,
                cancellationToken);

        var activationUrl =
            $"{_notificationOptions.FrontendBaseUrl}/activate-account" +
            $"?userId={notification.UserId}" +
            $"&token={Uri.EscapeDataString(activationToken)}";

        // Development/testing only.
        _logger.LogInformation(
            "ACTIVATION URL: {ActivationUrl}",
            activationUrl);

        var subject = "Activate your ILS account";

        var body = InternalUserCreatedEmailTemplate.Build(
            notification.Name,
            notification.Email,
            activationUrl);

        await _emailSender.SendAsync(
            notification.Email,
            subject,
            body,
            cancellationToken);

        _logger.LogInformation(
            "Activation email sent successfully to {Email}",
            notification.Email);
    }
}