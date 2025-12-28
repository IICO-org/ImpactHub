namespace ImpactHub.ERP.Shared.Navigation;

public sealed class NavigationMenu
{
    public string TenantCode { get; init; } = default!;
    public IReadOnlyList<NavigationModule> Modules { get; init; } = [];
}

public sealed class NavigationModule
{
    public string Code { get; init; } = default!;          // Projects, Donations
    public string DisplayName { get; init; } = default!;
    public string Icon { get; init; } = default!;
    public string Route { get; init; } = default!;
    public int Order { get; init; }

    public IReadOnlyList<NavigationItem> Items { get; init; } = [];
}

public sealed class NavigationItem
{
    public string Code { get; init; } = default!;
    public string DisplayName { get; init; } = default!;
    public string Route { get; init; } = default!;
    public string RequiredPermission { get; init; } = default!;
}
