using BuildingBlocks.Application;
using BuildingBlocks.Application.Behaviors;
using BuildingBlocks.Application.Contracts;
using FluentValidation;
using Identity.Application;
using Identity.Application.Abstractions;
using Identity.Application.SubAccounts.GetSubAccounts;
using Identity.Domain.Repositories;
using Identity.Infrastructure.Authentication;
using Identity.Infrastructure.Integrations;
using Identity.Infrastructure.Persistence;
using Identity.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Identity.Infrastructure;

public static class IdentityModuleServiceCollectionExtensions
{
    /// <summary>
    /// Registers everything the Identity module owns: its DbContext, ASP.NET Core Identity,
    /// JWT bearer authentication, repositories, and the module's own Options.
    /// Called once from Api/Program.cs — no other module touches these types directly.
    /// </summary>
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddOptions<Application.SubAccountOptions>()
            .Bind(configuration.GetSection(Identity.Application.SubAccountOptions.SectionName))
            .ValidateOnStart();

        services.AddOptions<Seeding.SeedOptions>()
            .Bind(configuration.GetSection(Seeding.SeedOptions.SectionName))
            .ValidateOnStart();

        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("IdentityDb")));



        services.AddScoped<IIdentityUnitOfWork>(
            sp => sp.GetRequiredService<IdentityDbContext>());

        services
            .AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            })
            .AddRoles<ApplicationRole>()
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidAudience = jwtSection["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSection["SigningKey"]!))
                };
            });

        services.AddAuthorizationBuilder();
        services.AddScoped<IAuthorizationHandler, Authorization.PermissionAuthorizationHandler>();

        //services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<IdentityDbContext>());

        services.AddScoped<ISubAccountRepository, SubAccountRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IInternalUserRepository, InternalUserRepository>();
        services.AddScoped<ISubAccountReadRepository,SubAccountReadRepository>();

        services.AddScoped<TokenClaimsBuilder>();

        services.AddScoped<IIdentityUserService, IdentityUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<Seeding.IdentitySeeder>();

        services.AddScoped<IIdentityUserRegistrar, IdentityUserRegistrar>();
        services.AddScoped<IIdentityUserUpdater, IdentityUserUpdater>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(
                typeof(IIdentityUserService).Assembly);

            cfg.AddOpenBehavior(
                typeof(ValidationBehavior<,>));
        });


        services.AddValidatorsFromAssembly(typeof(IIdentityUserService).Assembly);

        return services;
    }
}