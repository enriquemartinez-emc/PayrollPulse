using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email).HasMaxLength(320).IsRequired(false);
        builder.Property(u => u.Role).HasMaxLength(64).IsRequired();

        var converter = new ValueConverter<EmployeeId?, Guid?>(
            v => v.HasValue ? v.Value.Value : null,
            g => g.HasValue ? new EmployeeId(g.Value) : null
        );

        builder
            .Property(u => u.EmployeeId)
            .HasConversion(converter)
            .HasColumnName("employee_id")
            .IsRequired(false);

        builder.HasIndex(u => u.Email).IsUnique().HasFilter("\"email\" IS NOT NULL");

        builder
            .HasOne(u => u.Employee)
            .WithMany()
            .HasForeignKey(u => u.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
