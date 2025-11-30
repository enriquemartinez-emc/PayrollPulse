using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class PayrollPolicyConfiguration : IEntityTypeConfiguration<PayrollPolicy>
{
    public void Configure(EntityTypeBuilder<PayrollPolicy> builder)
    {
        builder.ToTable("payroll_policies");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).ValueGeneratedNever();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasColumnType("text");
        builder
            .Property(p => p.CompensationType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder
            .Property(p => p.CalculationType)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();
        builder.Property(p => p.RateOrAmount).HasPrecision(18, 4).IsRequired();
        builder.Property(p => p.IsActive).HasDefaultValue(true);

        builder.HasIndex(p => p.Name).IsUnique(false);
    }
}
