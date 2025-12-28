using Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Mappings;

internal sealed class RoleMap : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> b)
    {
        b.ToTable("Roles", "iam"); // الربط بجدولك في SQL
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("RoleId");
        b.Property(x => x.Code).HasMaxLength(80).IsRequired();
    }
}