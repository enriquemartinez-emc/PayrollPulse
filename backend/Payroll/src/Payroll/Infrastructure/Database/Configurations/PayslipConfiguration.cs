using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class PayslipConfiguration : IEntityTypeConfiguration<Payslip>
{
    public void Configure(EntityTypeBuilder<Payslip> builder)
    {
        builder.ToTable("payslips");

        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedNever();

        builder
            .Property(e => e.EmployeeId)
            .HasConversion(id => id.Value, value => new EmployeeId(value))
            .ValueGeneratedNever();

        builder.Property(p => p.EmployeeId).IsRequired();
        builder.Property(p => p.EmployeeName).IsRequired().HasMaxLength(200);

        builder.ComplexProperty(p => p.Period);

        builder.ComplexProperty(
            p => p.BaseSalary,
            mb =>
            {
                mb.Property(m => m.Amount)
                    .HasColumnName("base_salary_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();
                mb.Property(m => m.Currency)
                    .HasColumnName("base_salary_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            }
        );

        builder.ComplexProperty(
            p => p.TotalEarnings,
            mb =>
            {
                mb.Property(m => m.Amount)
                    .HasColumnName("total_earnings_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();
                mb.Property(m => m.Currency)
                    .HasColumnName("total_earnings_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            }
        );

        builder.ComplexProperty(
            p => p.TotalDeductions,
            mb =>
            {
                mb.Property(m => m.Amount)
                    .HasColumnName("total_deductions_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();
                mb.Property(m => m.Currency)
                    .HasColumnName("total_deductions_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            }
        );

        builder.ComplexProperty(
            p => p.NetPay,
            mb =>
            {
                mb.Property(m => m.Amount)
                    .HasColumnName("net_pay_amount")
                    .HasPrecision(18, 2)
                    .IsRequired();
                mb.Property(m => m.Currency)
                    .HasColumnName("net_pay_currency")
                    .HasMaxLength(3)
                    .IsRequired();
            }
        );
    }
}
