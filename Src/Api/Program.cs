using Api.BackgroundJobs;
using Api.Infrastructure.Email;
using Api.Infrastructure.ExceptionHandling;
using Api.Modules.Content;
//using Api.Modules.Customers;
using Api.Modules.Identity;
using Api.Modules.Notifications;
using Api.Modules.Reports;

using BuildingBlocks.Infrastructure;
using Content.Infrastructure;
using Customers.Infrastructure;
using Identity.Application;
using Identity.Application.Abstractions;
using Identity.Application.Auth.ForgotPassword;
using Identity.Infrastructure;
using Identity.Infrastructure.Seeding;
using Notifications.Application;
using Notifications.Infrastructure;
using Reports.Application;
using Reports.Infrastructure;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);


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


builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddIdentityApplication();
builder.Services.AddCustomersModule(builder.Configuration);
builder.Services.AddContentModule(builder.Configuration);
builder.Services.AddReportsModule(builder.Configuration);


builder.Services.AddBuildingBlocksInfrastructure();
builder.Services.AddReportsApplication();
builder.Services.AddNotificationsInfrastructure(builder.Configuration);
builder.Services.AddNotificationsApplication();

builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
builder.Services.AddExceptionHandler<ConflictExceptionHandler>();

builder.Services.AddProblemDetails(); // obligatory

builder.Services.AddHostedService<IdentityOutboxProcessorWorker>();
builder.Services.AddHostedService<CustomersOutboxProcessorWorker>();
builder.Services.AddHostedService<EmailOutboxProcessorWorker>();
builder.Services.AddScoped<IPasswordResetEmailSender, PasswordResetEmailSender>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();



builder.Services.Configure<ForgotPasswordOptions>(
    builder.Configuration.GetSection("Notifications"));

// Edited
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

}

app.UseExceptionHandler();

app.UseCors("Frontend");
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// Runs once (idempotent) — creates the 6 baseline Roles + the very first Super Admin
// account, solving the bootstrap problem (every other endpoint requires a permission).
using (var scope = app.Services.CreateScope())
{
    // Identity.Infrastructure.Seeding.
    var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
    await seeder.SeedAsync();
}

// Each module maps its own endpoint group. Adding a new module = one new line here.
app.MapIdentityEndpoints();
//app.MapCustomersEndpoints();
app.MapReportsEndpoints();

app.MapNotificationsEndpoints();
app.MapContentEndpoints();


app.Run();
