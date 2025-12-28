using Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Mappings;

internal sealed class UserMap : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        // الربط بالجدول الحقيقي (بدون Navigations عابرة للـ Schema)
        b.ToTable("Users", "iam");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("UserID"); // الربط باسم العمود الحقيقي في DB

        b.Property(x => x.ExternalId).HasColumnName("LoginID_Ms").IsRequired();
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.DisplayName).HasColumnName("DisplayNameEn").HasMaxLength(200);

        // أعمدة الـ Audit الموروثة من BaseEntity
        b.Property(x => x.CreatedAt).HasColumnType("datetime2").IsRequired();
        b.Property(x => x.CreatedBy).IsRequired();
    }
}
