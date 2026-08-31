using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notifications.Application.Abstractions;
using Notifications.Infrastructure.Email;
using Notifications.Infrastructure.Persistence;

namespace Notifications.Infrastructure;

public static class NotificationsModuleServiceCollectionExtensions
{
    public static IServiceCollection AddNotificationsInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)


    {
        services.AddDbContext<NotificationsDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("NotificationsDb")));

        services.AddOptions<EmailOptions>()
            .Configure(options =>
            {
                options.SmtpHost = configuration["Email:SmtpHost"] ?? "";
                options.SmtpPort = int.Parse(
                    configuration["Email:SmtpPort"] ?? "587");
                options.Username = configuration["Email:Username"] ?? "";
                options.Password = configuration["Email:Password"] ?? "";
                options.FromEmail = configuration["Email:FromEmail"] ?? "";
                options.FromName = configuration["Email:FromName"] ?? "";
            })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        return services;
    }
}