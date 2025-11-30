namespace Payroll.Infrastructure.Database;

public class PayrollDbContext : DbContext
{
    public PayrollDbContext(DbContextOptions<PayrollDbContext> options)
        : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<EmployeePayrollPolicy> EmployeePayrollPolicies => Set<EmployeePayrollPolicy>();
    public DbSet<PayrollRunFailure> PayrollRunFailures => Set<PayrollRunFailure>();
    public DbSet<PayrollRun> PayrollRuns => Set<PayrollRun>();
    public DbSet<Payslip> Payslips => Set<Payslip>();
    public DbSet<PayslipItem> PayslipItems => Set<PayslipItem>();
    public DbSet<PayrollPolicy> PayrollPolicies => Set<PayrollPolicy>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PayrollDbContext).Assembly);
    }
}
