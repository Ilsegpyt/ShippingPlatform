namespace Identity.Application;

/// <summary>Bound via the Options Pattern from appsettings (SubAccounts section).</summary>
public sealed class SubAccountOptions
{
    public const string SectionName = "SubAccounts";

    /// <summary>Temporary password assigned when a Customer Admin creates a new Sub-account.
    /// The sub-account is expected to change it on first login (enforced at the UI/flow level).</summary>
    public required string DefaultPassword { get; init; }
}
