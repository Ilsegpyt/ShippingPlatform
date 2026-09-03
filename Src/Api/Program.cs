using Api.BackgroundJobs;
using Api.Infrastructure.ExceptionHandling;
using Api.Modules.Customers;
using Api.Modules.Identity;
using Api.Modules.Reports;
using Api.Modules.Schedules;
using Api.Modules.Shipments;
using BuildingBlocks.Infrastructure;
using Customers.Infrastructure;
using Identity.Infrastructure;
using Identity.Infrastructure.Seeding;
using Notifications.Application;
using Notifications.Infrastructure;
using Reports.Application;
using Reports.Infrastructure;
using Schedules.Application;
using Schedules.Infrastructure;
using Shipments.Application;
using Shipments.Infrastructure;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityModule(builder.Configuration);
builder.Services.AddCustomersModule(builder.Configuration);
builder.Services.AddReportsModule(builder.Configuration);
builder.Services.AddSchedulesInfrastructure(builder.Configuration);
builder.Services.AddShipmentsInfrastructure(builder.Configuration);
builder.Services.AddShipmentsApplication();

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

builder.Services.AddSchedulesApplication();
builder.Services.AddSchedulesInfrastructure(
    builder.Configuration);



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
app.MapSchedulesEndpoints();
app.MapShipmentsEndpoints();

app.Run();
