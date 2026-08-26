namespace Identity.Infrastructure.Seeding;

/// <summary>
/// Bootstrap-only configuration. Solves the "chicken-and-egg" problem: every endpoint
/// requires a permission, so the very first Super Admin cannot be created through the API.
/// The seeder reads this once on startup and creates that first account directly.
/// </summary>
public sealed class SeedOptions
{
    public const string SectionName = "Seed";

    public required string SuperAdminEmail { get; init; }
    public required string SuperAdminPassword { get; init; }
}