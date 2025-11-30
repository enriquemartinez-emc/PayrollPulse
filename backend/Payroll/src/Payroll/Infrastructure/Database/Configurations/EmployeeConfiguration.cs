using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("employees");

        builder.HasKey(e => e.EmployeeId);
        builder
            .Property(e => e.EmployeeId)
            .HasConversion(id => id.Value, value => new EmployeeId(value))
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.LastName).IsRequired().HasMaxLength(100);
        builder.Property(e => e.Email).IsRequired().HasMaxLength(320);

        builder.ComplexProperty(e => e.BaseSalary);

        builder.Property(e => e.HireDate).IsRequired();
        builder.Property(e => e.TerminationDate).IsRequired(false);

        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
    }
}
