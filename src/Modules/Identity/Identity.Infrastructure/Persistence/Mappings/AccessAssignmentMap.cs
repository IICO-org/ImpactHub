using Identity.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Identity.Infrastructure.Persistence.Mappings;

internal sealed class AccessAssignmentMap : IEntityTypeConfiguration<AccessAssignment>
{
    public void Configure(EntityTypeBuilder<AccessAssignment> b)
    {
        b.ToTable("AccessAssignment", "iam");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).HasColumnName("AssignmentId");
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
    }
}