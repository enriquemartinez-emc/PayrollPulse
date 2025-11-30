using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class EmployeePayrollPolicyConfiguration
    : IEntityTypeConfiguration<EmployeePayrollPolicy>
{
    public void Configure(EntityTypeBuilder<EmployeePayrollPolicy> builder)
    {
        builder.ToTable("employee_payroll_policies");
        builder.HasKey(ep => ep.Id);

        builder
            .Property(ep => ep.EmployeeId)
            .HasConversion(id => id.Value, value => new EmployeeId(value))
            .HasColumnName("employee_id")
            .IsRequired();

        builder.Property(ep => ep.PayrollPolicyId).IsRequired();
        builder.Property(ep => ep.OverrideRateOrAmount);
        builder.Property(ep => ep.IsActive).IsRequired();
        builder.Property(ep => ep.EffectiveFrom);
        builder.Property(ep => ep.EffectiveTo);

        builder
            .HasOne(ep => ep.PayrollPolicy)
            .WithMany()
            .HasForeignKey(ep => ep.PayrollPolicyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
