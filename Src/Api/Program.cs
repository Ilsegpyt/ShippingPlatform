using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Api.BackgroundJobs;
using Api.Infrastructure;
using Api.Infrastructure.Email;
using Api.Infrastructure.ExceptionHandling;
using Api.Modules.Content;
using Api.Modules.Customers;
using Api.Modules.Identity;
using Api.Modules.Notifications;
using Api.Modules.Operations;
using Api.Modules.Reports;
using Api.Modules.Website;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Infrastructure;
using Content.Infrastructure;
using Customers.Infrastructure;
using Identity.Application;
using Identity.Application.Abstractions;
using Identity.Application.Auth.ForgotPassword;
using Identity.Infrastructure;
using Identity.Infrastructure.Seeding;
using Microsoft.AspNetCore.RateLimiting;
using Notifications.Application;
using Notifications.Infrastructure;
using Operations.Application;
using Operations.Infrastructure;
using Reports.Application;
using Reports.Infrastructure;
using Website.Application;
using Website.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ==================================================
// Logging
// ==================================================
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// ==================================================
// CORS
// ==================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "http://localhost:5174"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ==================================================
// Rate Limiting
// ==================================================
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode =
        StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, cancellationToken) =>
    {
        var logger = context.HttpContext.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("RateLimiting");

        var endpoint = context.HttpContext.GetEndpoint();

        var policyName = endpoint?.Metadata
            .GetMetadata<EnableRateLimitingAttribute>()
            ?.PolicyName ?? "GlobalLimiter";

        logger.LogWarning(
            "RATE LIMIT REJECTED | Method: {Method} | Path: {Path} | IP: {IP} | Policy: {Policy}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            context.HttpContext.Connection.RemoteIpAddress?.ToString(),
            policyName
        );

        context.HttpContext.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        await Task.CompletedTask;
    };

    // Global limit: زيادة الليميت لـ 250 وإلغاء الـ Queue لمنع البطء وتجنب الـ 429
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                var ipAddress =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetFixedWindowLimiter(
                    ipAddress,
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 250,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    });
            });

    // Login limit: 5 requests per minute per IP
    options.AddPolicy("LoginPolicy", httpContext =>
    {
        var ipAddress =
            httpContext.Connection.RemoteIpAddress?.ToString()
            ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            ipAddress,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });
});

// ==================================================
// Identity & Customers
// ==================================================
builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddCustomersModule(builder.Configuration);

// ==================================================
// Content & Reports
// ==================================================
builder.Services.AddContentModule(builder.Configuration);
builder.Services.AddReportsModule(builder.Configuration);
builder.Services.AddReportsApplication();

// ==================================================
// Operations
// ==================================================
builder.Services.AddOperationsInfrastructure(builder.Configuration);
builder.Services.AddOperationsApplication();

// ==================================================
// Notifications
// ==================================================
builder.Services.AddNotificationsInfrastructure(builder.Configuration);
builder.Services.AddNotificationsApplication();

// ==================================================
// Website
// ==================================================
builder.Services.AddWebsiteApplication();
builder.Services.AddWebsiteModule(builder.Configuration);

// ==================================================
// HTTP Context & Current User
// ==================================================
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// ==================================================
// Exception Handling
// ==================================================
builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
builder.Services.AddExceptionHandler<ConflictExceptionHandler>();
builder.Services.AddProblemDetails();

// ==================================================
// Background Jobs & Email
// ==================================================
builder.Services.AddHostedService<IdentityOutboxProcessorWorker>();
builder.Services.AddHostedService<CustomersOutboxProcessorWorker>();
builder.Services.AddHostedService<EmailOutboxProcessorWorker>();
builder.Services.AddScoped<IPasswordResetEmailSender, PasswordResetEmailSender>();

// ==================================================
// OpenAPI
// ==================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// ==================================================
// Application Options
// ==================================================
builder.Services.Configure<ForgotPasswordOptions>(
    builder.Configuration.GetSection("Notifications"));
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

// ==================================================
// Build Application
// ==================================================
var app = builder.Build();

// ==================================================
// OpenAPI
// ==================================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// ==================================================
// Middleware Pipeline
// ==================================================
app.UseExceptionHandler();
app.UseRouting();
app.UseCors("Frontend");
app.UseStaticFiles();

// Log incoming requests and their response status codes
app.Use(async (context, next) =>
{
    var logger = context.RequestServices
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("RequestLogging");

    var path = context.Request.Path.Value ?? string.Empty;

    if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
    {
        logger.LogInformation(
            "API REQUEST START | {Method} {Path} | IP: {IP}",
            context.Request.Method,
            context.Request.Path,
            context.Connection.RemoteIpAddress?.ToString());
    }

    await next();

    if (path.StartsWith("/api", StringComparison.OrdinalIgnoreCase))
    {
        logger.LogInformation(
            "API REQUEST END | {Method} {Path} | Status: {StatusCode}",
            context.Request.Method,
            context.Request.Path,
            context.Response.StatusCode);
    }
});

app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// ==================================================
// Database Seeding
// ==================================================
using (var scope = app.Services.CreateScope())
{
    var seeder =
        scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

    await seeder.SeedAsync();
}

// ==================================================
// Endpoint Mapping
// ==================================================
app.MapIdentityEndpoints();
app.MapCustomersEndpoints();
app.MapReportsEndpoints();
app.MapNotificationsEndpoints();
app.MapContentEndpoints();
app.MapOperationsEndpoints();
WebsiteEndpoints.Map(app);

// ==================================================
// Run Application
// ==================================================
app.Run();