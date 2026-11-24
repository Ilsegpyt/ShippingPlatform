
using System.Security.Claims;
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
using Api.Modules.Transporters;
using Api.Modules.Website;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Serialization;
using BuildingBlocks.Infrastructure;
using Content.Infrastructure;
using Customers.Infrastructure;
using Identity.Application;
using Identity.Application.Abstractions;
using Identity.Application.Auth.ForgotPassword;
using Identity.Infrastructure;
using Identity.Infrastructure.Seeding;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Notifications.Application;
using Notifications.Infrastructure;
using Operations.Application;
using Operations.Infrastructure;
using Reports.Application;
using Reports.Infrastructure;
using Transporters.Application;
using Transporters.Infrastructure;
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
        if (builder.Environment.IsDevelopment())
        {
            policy
                .WithOrigins(
                    "http://localhost:5173",
                    "http://localhost:5174",
                    "http://localhost",
                    "http://127.0.0.1:5173"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
        else
        {
            policy
                .WithOrigins(
                    "https://ilsegypt.com",
                    "https://www.ilsegypt.com"
                )
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        }
    });
});


// ==================================================
// Forwarded Headers (Reverse Proxy / Nginx Support)
// ==================================================
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
    options.ForwardLimit = null; // Accept multiple hops (e.g. Cloudflare -> Nginx -> Kestrel)
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

        var clientIdentifier = context.HttpContext.User?.Identity?.IsAuthenticated == true
            ? $"User:{context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.HttpContext.User.Identity?.Name}"
            : $"IP:{context.HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

        logger.LogWarning(
            "RATE LIMIT REJECTED | Method: {Method} | Path: {Path} | Client: {Client} | Policy: {Policy}",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path,
            clientIdentifier,
            policyName
        );

        context.HttpContext.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        context.HttpContext.Response.ContentType =
            "application/json";

        context.HttpContext.Response.Headers.RetryAfter = "3";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            message = "You have exceeded the allowed number of requests. Please try again in a few seconds."
        }, cancellationToken);
    };

    // Global limiter with token bucket & concurrency queue
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(
            httpContext =>
            {
                // 1. Never rate-limit CORS preflight OPTIONS requests
                if (HttpMethods.IsOptions(httpContext.Request.Method))
                {
                    return RateLimitPartition.GetNoLimiter("OPTIONS");
                }

                // 2. Authenticated user: partition by UserId with generous burst capacity for dashboards & multi-tabs
                if (httpContext.User?.Identity?.IsAuthenticated == true)
                {
                    var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                                 ?? httpContext.User.Identity?.Name
                                 ?? "authenticated_user";

                    return RateLimitPartition.GetTokenBucketLimiter(
                        $"user_{userId}",
                        _ => new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 150,                            // High burst capacity (safely supports 8+ simultaneous dashboard tabs)
                            TokensPerPeriod = 40,                        // Rapid replenishment (1,200 req/min continuous)
                            ReplenishmentPeriod = TimeSpan.FromSeconds(2),
                            QueueLimit = 50,                             // Gracefully queues unexpected spikes instead of 429
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            AutoReplenishment = true
                        });
                }

                // 3. Anonymous / Public requests: partition by Client IP
                var ipAddress =
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown";

                return RateLimitPartition.GetTokenBucketLimiter(
                    $"ip_{ipAddress}",
                    _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 50,
                        TokensPerPeriod = 20,
                        ReplenishmentPeriod = TimeSpan.FromSeconds(3),
                        QueueLimit = 25,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
            });

    // Login limit: 5 requests per minute per IP to protect against brute-force attacks
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
// Transporters
// ==================================================
builder.Services.AddTransportersApplication();
builder.Services.AddTransportersModule(builder.Configuration);

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
    options.SerializerOptions.Converters.Add(
        new SafeStringDictionaryConverter());
});

builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
    options.JsonSerializerOptions.Converters.Add(
        new SafeStringDictionaryConverter());
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
app.UseForwardedHeaders();
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

app.UseAuthentication();
app.UseAuthorization();

// Rate Limiting Middleware (placed after Authentication/Authorization so User identity is known)
app.UseRateLimiter();

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
TransportersEndpoints.Map(app);

// ==================================================
// Run Application
// ==================================================
app.Run();
