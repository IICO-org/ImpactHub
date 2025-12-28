using Identity.Domain;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence;

public sealed class IamDbContext : DbContext
{
    public IamDbContext(DbContextOptions<IamDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<AccessAssignment> AccessAssignments => Set<AccessAssignment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1. تحديد الـ Schema الافتراضي
        modelBuilder.HasDefaultSchema("iam");

        // 2. مطابقة الكيانات مع جداول قاعدة البيانات وأسماء الـ IDs التي اخترتها

        // جدول AccessAssignment
        modelBuilder.Entity<AccessAssignment>(entity =>
        {
            entity.ToTable("AccessAssignment");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("AssignmentID"); // الربط مع الاسم الجديد
            entity.Property(e => e.UserId).HasColumnName("UserId");
            entity.Property(e => e.RoleId).HasColumnName("RoleId");
        });

        // جدول Users
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("UserID"); // الربط مع UserID في SQL
        });

        // جدول Roles
        modelBuilder.Entity<Role>(entity =>
        {
            entity.ToTable("Roles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("RoleID"); // الربط مع RoleID في SQL
        });

        // 3. تطبيق أي إعدادات أخرى موجودة في ملفات منفصلة (اختياري)
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IamDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}