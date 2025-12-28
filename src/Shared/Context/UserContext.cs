namespace ImpactHub.ERP.SharedKernel
{
    public sealed class UserContext
    {
        public int UserId { get; init; }
        public Guid AzureObjectId { get; init; }
        public string Email { get; init; } = default!;

        public bool IsActive { get; init; }
        public bool IsSystemAdmin { get; init; }

        public IReadOnlyCollection<string> Roles { get; init; } = [];
        public IReadOnlyCollection<string> Permissions { get; init; } = [];
    }
}

