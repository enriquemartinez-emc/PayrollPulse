using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class PayslipItemConfiguration : IEntityTypeConfiguration<PayslipItem>
{
    public void Configure(EntityTypeBuilder<PayslipItem> builder)
    {
        builder.ToTable("payslip_items");

        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedNever();

        builder.Property<Guid>(i => i.PayslipId).HasColumnName("payslip_id").IsRequired();

        builder
            .HasOne(pi => pi.PayrollPolicy)
            .WithMany()
            .HasForeignKey(pi => pi.PolicyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(i => i.CompensationType)
            .HasColumnName("compensation_type")
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.ComplexProperty(
            p => p.Amount,
            mb =>
            {
                mb.Property(m => m.Amount).HasColumnName("amount").HasPrecision(18, 2).IsRequired();
                mb.Property(m => m.Currency).HasColumnName("currency").HasMaxLength(3).IsRequired();
            }
        );
    }
}
