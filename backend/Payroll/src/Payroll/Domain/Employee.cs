using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;

namespace Payroll.Domain;

public sealed class Employee
{
    private readonly List<EmployeePayrollPolicy> _employeePayrollPolicies = [];
    private readonly List<Payslip> _payslips = [];

    public EmployeeId EmployeeId { get; }
    public string FirstName { get; } = string.Empty;
    public string LastName { get; } = string.Empty;
    public string Email { get; } = string.Empty;
    public Money BaseSalary { get; private set; }
    public EmploymentStatus Status { get; private set; }

    public Guid DepartmentId { get; private set; }
    public Department? Department { get; private set; }

    public DateOnly HireDate { get; private set; }
    public DateOnly? TerminationDate { get; private set; }

    public IReadOnlyCollection<EmployeePayrollPolicy> EmployeePayrollPolicies =>
        _employeePayrollPolicies.AsReadOnly();
    public IReadOnlyCollection<Payslip> Payslips => _payslips.AsReadOnly();
    public string FullName => $"{FirstName} {LastName}";

    private Employee() { } // for EF Core

    private Employee(
        EmployeeId id,
        string firstName,
        string lastName,
        string email,
        Money baseSalary,
        Department department,
        EmploymentStatus status,
        DateOnly hireDate
    )
    {
        EmployeeId = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        BaseSalary = baseSalary;
        Department = department;
        Status = status;
        HireDate = hireDate;
    }

    public static Result<Employee> Create(
        EmployeeId id,
        string firstName,
        string lastName,
        string email,
        Money baseSalary,
        Department department,
        DateOnly hireDate
    )
    {
        var employee = new Employee(
            id,
            firstName,
            lastName,
            email,
            baseSalary,
            department,
            EmploymentStatus.Active,
            hireDate
        );

        return Result.Success(employee);
    }

    public Result Terminate(DateOnly terminationDate)
    {
        if (terminationDate < HireDate)
            return Result.Failure(Errors.Employee.TerminationBeforeHire(EmployeeId));

        Status = EmploymentStatus.Terminated;
        TerminationDate = terminationDate;
        return Result.Success();
    }

    public Result AdjustSalary(decimal newAmount)
    {
        if (newAmount <= 0)
            return Result.Failure(Errors.Employee.InvalidSalary(EmployeeId));

        BaseSalary = new Money(newAmount);
        return Result.Success();
    }

    public bool IsActiveDuring(PayPeriod period)
    {
        if (Status == EmploymentStatus.Terminated && TerminationDate.HasValue)
            return !(TerminationDate.Value < period.Start);

        return HireDate <= period.End;
    }

    public Result AssignPolicy(EmployeePayrollPolicy assignment)
    {
        if (!assignment.EmployeeId.Value.Equals(EmployeeId.Value))
            return Result.Failure(Errors.Employee.PolicyAssignmentMismatch(EmployeeId));

        if (
            _employeePayrollPolicies.Any(p =>
                p.PayrollPolicyId == assignment.PayrollPolicyId && p.IsActive
            )
        )
            return Result.Failure(
                Errors.Employee.PolicyAlreadyAssigned(EmployeeId, assignment.PayrollPolicyId)
            );

        _employeePayrollPolicies.Add(assignment);
        return Result.Success();
    }

    public void ChangeState(EmploymentStatus newState) => Status = newState;
}

public enum EmploymentStatus
{
    Active,
    Terminated,
    OnLeave,
}
