using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class PayrollRunConfiguration : IEntityTypeConfiguration<PayrollRun>
{
    public void Configure(EntityTypeBuilder<PayrollRun> builder)
    {
        builder.ToTable("payroll_runs");
        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.CreatedAtUtc).IsRequired();

        builder.ComplexProperty(
            p => p.Period,
            pp =>
            {
                pp.Property(p => p.Start).HasColumnName("period_start");
                pp.Property(p => p.End).HasColumnName("period_end");
            }
        );

        builder.ComplexProperty(pr => pr.TotalGross);
        builder.ComplexProperty(pr => pr.TotalNet);
        builder.ComplexProperty(pr => pr.TotalBonuses);
        builder.ComplexProperty(pr => pr.TotalDeductions);
    }
}
