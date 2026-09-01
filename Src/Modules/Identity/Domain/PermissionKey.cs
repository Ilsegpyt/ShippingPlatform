using BuildingBlocks.Domain;

namespace Identity.Domain;

/// <summary>
/// A single granular permission, e.g. "shipments.view", "documents.upload".
/// Kept as a typed Value Object (not a raw string) so callers can't pass an arbitrary
/// unvalidated string where a permission key is expected.
/// </summary>
public sealed class PermissionKey : ValueObject
{
    public string Value { get; }

    private PermissionKey(string value) => Value = value;

    public static PermissionKey Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException(
                "Permission key cannot be empty.",
                nameof(value));

        return new PermissionKey(
            value.Trim().ToLowerInvariant());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}

/// <summary>
/// Central catalog of known permission keys.
/// </summary>
public static class PermissionCatalog
{
    // Account Manager Assignment
    public static readonly PermissionKey CustomersAssignAccountManager =
        PermissionKey.Of("customers.accountmanager.assign");


    // Identity
    public static readonly PermissionKey UsersCreate =
        PermissionKey.Of("identity.users.create");

    public static readonly PermissionKey UsersEdit =
        PermissionKey.Of("identity.users.edit");

    public static readonly PermissionKey UsersSuspend =
        PermissionKey.Of("identity.users.suspend");

    public static readonly PermissionKey UsersDelete =
        PermissionKey.Of("identity.users.delete");

    public static readonly PermissionKey RolesManage =
        PermissionKey.Of("identity.roles.manage");


    // SubAccounts
    public static readonly PermissionKey SubAccountsView =
        PermissionKey.Of("identity.subaccounts.view");

    public static readonly PermissionKey SubAccountsCreate =
        PermissionKey.Of("identity.subaccounts.create");

    public static readonly PermissionKey SubAccountsEdit =
        PermissionKey.Of("identity.subaccounts.edit");

    public static readonly PermissionKey SubAccountsDelete =
        PermissionKey.Of("identity.subaccounts.delete");

    public static readonly PermissionKey SubAccountsSuspend =
        PermissionKey.Of("identity.subaccounts.suspend");


    // Customers
    public static readonly PermissionKey CustomersView =
        PermissionKey.Of("customers.view");

    public static readonly PermissionKey CustomersCreate =
        PermissionKey.Of("customers.create");

    public static readonly PermissionKey CustomersEdit =
        PermissionKey.Of("customers.edit");

    public static readonly PermissionKey CustomersSuspend =
        PermissionKey.Of("customers.suspend");

    public static readonly PermissionKey CustomersDelete =
        PermissionKey.Of("customers.delete");

    public static readonly PermissionKey CustomersImpersonate =
        PermissionKey.Of("customers.impersonate");


    // Shipments
    public static readonly PermissionKey ShipmentsView =
        PermissionKey.Of("shipments.view");

    public static readonly PermissionKey ShipmentsEdit =
        PermissionKey.Of("shipments.edit");

    public static readonly PermissionKey ShipmentsDelete =
        PermissionKey.Of("shipments.delete");

    public static readonly PermissionKey ShipmentsBook =
        PermissionKey.Of("shipments.book");

    public static readonly PermissionKey ShipmentsTrack =
        PermissionKey.Of("shipments.track");





    // Reports
    public static readonly PermissionKey ReportsView =
        PermissionKey.Of("reports.view");

    public static readonly PermissionKey ReportsUpload =
        PermissionKey.Of("reports.upload");


    // Schedules
    public static readonly PermissionKey SchedulesView =
        PermissionKey.Of("schedules.view");

    public static readonly PermissionKey SchedulesCreate =
        PermissionKey.Of("schedules.create");

    public static readonly PermissionKey SchedulesDelete =
        PermissionKey.Of("schedules.delete");

    public static readonly PermissionKey SchedulesImport =
        PermissionKey.Of("schedules.import");

    public static readonly PermissionKey SchedulesExport =
        PermissionKey.Of("schedules.export");


    public static readonly PermissionKey SchedulesUpdate =
        PermissionKey.Of("schedules.update");


    // Permissions available to SubAccounts
    public static readonly IReadOnlyCollection<PermissionKey> SubAccountPermissions =
    [
       
        ReportsView
    ];

      public static readonly IReadOnlyCollection<PermissionKey> AccountManagerPermissions =
      [
      ReportsUpload
      ];


    // Permissions available to Customer Owners
    public static readonly IReadOnlyCollection<PermissionKey> CustomerOwnerPermissions =
    [
        CustomersView,
        CustomersEdit,
        CustomersDelete,

        SubAccountsView,
        SubAccountsCreate,
        SubAccountsEdit,
        SubAccountsDelete,
        SubAccountsSuspend,

        ShipmentsView,
        ShipmentsEdit,
        ShipmentsBook,
        ShipmentsTrack,

        ReportsView,

        SchedulesView,
        SchedulesCreate,
        SchedulesDelete,
        SchedulesImport,
        SchedulesExport
    ];


    // All known permissions
    public static IReadOnlyCollection<PermissionKey> All { get; } = new[]
    {
        CustomersAssignAccountManager,

        // Identity
        UsersCreate,
        UsersEdit,
        UsersSuspend,
        UsersDelete,
        RolesManage,

        // SubAccounts
        SubAccountsView,
        SubAccountsCreate,
        SubAccountsEdit,
        SubAccountsDelete,
        SubAccountsSuspend,

        // Customers
        CustomersView,
        CustomersCreate,
        CustomersEdit,
        CustomersSuspend,
        CustomersDelete,
        CustomersImpersonate,

        // Shipments
        ShipmentsView,
        ShipmentsEdit,
        ShipmentsDelete,
        ShipmentsBook,
        ShipmentsTrack,

        // Documents
        //DocumentsView,
        //DocumentsUpload,

        // Reports
        ReportsView,
        ReportsUpload,

        // Schedules
        SchedulesView,
        SchedulesCreate,
        SchedulesDelete,
        SchedulesImport,
        SchedulesExport,
        SchedulesUpdate
    };
}