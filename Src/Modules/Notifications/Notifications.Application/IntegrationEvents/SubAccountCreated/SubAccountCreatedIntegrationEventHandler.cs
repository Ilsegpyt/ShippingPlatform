using BuildingBlocks.Contracts.IntegrationEvents.Identity;
using Identity.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Notifications.Application.Abstractions;
using Notifications.Application.Options;
using Notifications.Application.Templates.Emails;

namespace Notifications.Application.IntegrationEvents.SubAccountCreated;

public sealed class SubAccountCreatedIntegrationEventHandler
    : INotificationHandler<SubAccountCreatedIntegrationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<SubAccountCreatedIntegrationEventHandler> _logger;
    private readonly IActivationService _activationService;
    private readonly NotificationOptions _notificationOptions;

    public SubAccountCreatedIntegrationEventHandler(
        IEmailSender emailSender,
        IActivationService activationService,
        IOptions<NotificationOptions> notificationOptions,
        ILogger<SubAccountCreatedIntegrationEventHandler> logger)
    {
        _emailSender = emailSender;
        _activationService = activationService;
        _notificationOptions = notificationOptions.Value;
        _logger = logger;
    }

    public async Task Handle(
        SubAccountCreatedIntegrationEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling SubAccountCreatedIntegrationEvent for {Email}",
            notification.Email);

        var activationToken =
            await _activationService.GenerateActivationTokenAsync(
                notification.UserId,
                cancellationToken);

        var activationUrl =
            $"{_notificationOptions.FrontendBaseUrl}/activate-account" +
            $"?userId={notification.UserId}" +
            $"&token={Uri.EscapeDataString(activationToken)}";



        _logger.LogInformation(
            "Activation URL generated for {Email}: {ActivationUrl}",
            notification.Email,
            activationUrl);



        var subject = "Welcome to ILS";

        var body = SubAccountCreatedEmailTemplate.Build(
            notification.Name,
            notification.Email,
            activationUrl);

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