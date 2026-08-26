using BuildingBlocks.Domain;
using Identity.Domain.Exceptions;

namespace Identity.Domain;

/// <summary>
/// Encapsulates the business rule that binds Category -> allowed Service(s) -> allowed Type(s).
/// A SubAccount can hold MANY of these (additive), e.g. "Air/Freight/Import" + "Sea/Freight/Export".
///
/// Rules (as defined by the business):
///   Sea       -> Service = Freight (implicit), Type = All/Import/Export
///   Air       -> Service = Freight (implicit), Type = All/Import/Export
///   Domestic  -> Service = CustomsClearance/Transportation/Both, Type = All/Import/Export
///   Financial -> no Service, no Type
///
/// Full access is controlled by SubAccount.ScopeType = Full.
/// When ScopeType is Full, scope checks are skipped because the sub-account
/// has access to everything.
/// </summary>
public sealed class PermissionScope : ValueObject
{
    public ScopeCategory Category { get; }
    public ScopeService Service { get; }
    public ScopeShipmentType Type { get; }

    private PermissionScope(ScopeCategory category, ScopeService service, ScopeShipmentType type)
    {
        Category = category;
        Service = service;
        Type = type;
    }

    public static PermissionScope Create(ScopeCategory category, ScopeService service, ScopeShipmentType type)
    {
        switch (category)
        {
            case ScopeCategory.Financial:
                if (service != ScopeService.None || type != ScopeShipmentType.None)
                    throw new InvalidScopeCombinationException(
                        "Category 'Financial' does not accept a Service or Type.");
                break;

            case ScopeCategory.Sea:
            case ScopeCategory.Air:
                if (service != ScopeService.Freight)
                    throw new InvalidScopeCombinationException(
                        $"Category '{category}' only accepts Service = Freight.");
                RequireShipmentType(type, category);
                break;

            case ScopeCategory.Domestic:
                if (service is not (ScopeService.CustomsClearance or ScopeService.Transportation or ScopeService.Both))
                    throw new InvalidScopeCombinationException(
                        "Category 'Domestic' only accepts Service = CustomsClearance, Transportation, or Both.");
                RequireShipmentType(type, category);
                break;

            default:
                throw new InvalidScopeCombinationException($"Unknown category '{category}'.");
        }

        return new PermissionScope(category, service, type);
    }

    // Edited
    #region CreateFreight / Financial

    /// <summary>Convenience factory: Sea/Air auto-assign Service = Freight.</summary>
    //public static PermissionScope CreateFreight(ScopeCategory category, ScopeShipmentType type)
    //{
    //    if (category is not (ScopeCategory.Sea or ScopeCategory.Air))
    //        throw new InvalidScopeCombinationException("CreateFreight only applies to Sea or Air.");
    //    return Create(category, ScopeService.Freight, type);
    //}

    //public static PermissionScope Financial() =>
    //    new(ScopeCategory.Financial, ScopeService.None, ScopeShipmentType.None); 
    #endregion

    private static void RequireShipmentType(ScopeShipmentType type, ScopeCategory category)
    {
        if (type is not (ScopeShipmentType.All or ScopeShipmentType.Import or ScopeShipmentType.Export))
            throw new InvalidScopeCombinationException(
                $"Category '{category}' requires Type = All, Import, or Export.");
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Category;
        yield return Service;
        yield return Type;
    }
}