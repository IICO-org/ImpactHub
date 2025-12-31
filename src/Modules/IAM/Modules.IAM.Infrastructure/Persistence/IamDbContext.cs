using Microsoft.EntityFrameworkCore;

namespace Modules.IAM.Infrastructure.Persistence;

public sealed class IamDbContext : DbContext
{
    public IamDbContext(DbContextOptions<IamDbContext> options) : base(options) { }

    public DbSet<UserRow> Users => Set<UserRow>();
    public DbSet<UserIdentityRow> UserIdentities => Set<UserIdentityRow>();
    public DbSet<TenantRow> Tenants => Set<TenantRow>();
    public DbSet<AccessAssignmentRow> AccessAssignments => Set<AccessAssignmentRow>();
    public DbSet<RolePermissionRow> RolePermissions => Set<RolePermissionRow>();
    public DbSet<PermissionRow> Permissions => Set<PermissionRow>();

    // ===== Added for Access Profile =====
    public DbSet<RoleRow> Roles => Set<RoleRow>();
    public DbSet<FeatureRow> Features => Set<FeatureRow>();
    public DbSet<TenantFeatureRow> TenantFeatures => Set<TenantFeatureRow>();
    // ====================================

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRow>()
            .ToTable("Users", "iam")
            .HasKey(x => x.UserId);

        modelBuilder.Entity<UserIdentityRow>()
            .ToTable("UserIdentities", "iam")
            .HasKey(x => x.UserIdentityId);

        modelBuilder.Entity<TenantRow>()
            .ToTable("Tenants", "iam")
            .HasKey(x => x.TenantId);

        modelBuilder.Entity<AccessAssignmentRow>()
            .ToTable("AccessAssignment", "iam")
            .HasKey(x => x.AssignmentId);

        modelBuilder.Entity<RolePermissionRow>()
            .ToTable("RolePermission", "iam")
            .HasKey(x => new { x.RoleId, x.PermissionId });

        modelBuilder.Entity<PermissionRow>()
            .ToTable("Permissions", "iam")
            .HasKey(x => x.PermissionId);

        // ===== Added for Access Profile =====
        modelBuilder.Entity<RoleRow>()
            .ToTable("Roles", "iam")
            .HasKey(x => x.RoleId);

        modelBuilder.Entity<FeatureRow>()
            .ToTable("Features", "iam")
            .HasKey(x => x.FeatureId);

        modelBuilder.Entity<TenantFeatureRow>()
            .ToTable("TenantFeatures", "iam")
            .HasKey(x => new { x.TenantId, x.FeatureId });
        // ====================================

        base.OnModelCreating(modelBuilder);
    }
}
