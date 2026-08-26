namespace Identity.Domain;
/// <summary>
/// Valid values depend on ScopeCategory:
/// Sea/Air        -> Freight only (auto-assigned)
/// Domestic       -> CustomsClearance / Transportation / Both
/// AllCategories  -> None
/// Financial      -> None
/// </summary>
public enum ScopeCategory
{
    Sea = 1,
    Air = 2,
    Domestic = 3,
    Financial = 4
}


public enum ScopeService
{
    None = 0,
    Freight = 1,
    CustomsClearance = 2,
    Transportation = 3,
    Both = 4
}

public enum ScopeShipmentType
{
    None = 0,
    All = 1,
    Import = 2,
    Export = 3
}

public enum ScopeType
{
    Full = 0,
    Custom = 1
}

public enum SubAccountStatus
{
    Active = 0,
    Inactive = 1
}
