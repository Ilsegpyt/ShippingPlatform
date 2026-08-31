using Api.BackgroundJobs;
using Api.Infrastructure.ExceptionHandling;
using Api.Modules.Customers;
using Api.Modules.Identity;
using BuildingBlocks.Infrastructure;
using Customers.Infrastructure;
using Identity.Infrastructure;
using Identity.Infrastructure.Seeding;
using Notifications.Application;
using Notifications.Infrastructure;
using System.Text.Json.Serialization;
using Reports.Infrastructure;
using Api.Modules.Reports;
using Reports.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCustomersModule(builder.Configuration);
builder.Services.AddReportsModule(builder.Configuration);
builder.Services.AddBuildingBlocksInfrastructure();
builder.Services.AddReportsApplication();
builder.Services.AddNotificationsInfrastructure(builder.Configuration);
builder.Services.AddNotificationsApplication();

builder.Services.AddExceptionHandler<FluentValidationExceptionHandler>();
builder.Services.AddExceptionHandler<ConflictExceptionHandler>();

builder.Services.AddProblemDetails(); // obligatory

builder.Services.AddHostedService<OutboxProcessorWorker>();
builder.Services.AddHostedService<CustomersOutboxProcessorWorker>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

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
app.MapCustomersEndpoints();
app.MapReportsEndpoints(); 
app.Run();
