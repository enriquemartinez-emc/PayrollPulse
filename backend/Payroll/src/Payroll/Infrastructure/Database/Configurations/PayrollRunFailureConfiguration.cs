using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class PayrollRunFailureConfiguration : IEntityTypeConfiguration<PayrollRunFailure>
{
    public void Configure(EntityTypeBuilder<PayrollRunFailure> builder)
    {
        builder.ToTable("payroll_run_failures");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Code).HasMaxLength(200);
        builder.Property(x => x.Message).HasColumnType("text");
        builder
            .Property(x => x.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .HasDefaultValueSql("now()");
        builder.HasIndex(x => x.PayrollRunId);
    }
}
