using Payroll.Domain.ValueObjects;

namespace Payroll.Domain.Common;

public sealed record DomainError(string Code, string Message)
{
    public static DomainError Create(string code, string message) => new(code, message);

    public override string ToString() => $"{Code}: {Message}";
}

public static class Errors
{
    public static class Employee
    {
        public static DomainError NotFound(Guid id) =>
            DomainError.Create("Employee.NotFound", $"Employee with ID '{id}' was not found.");

        public static DomainError InvalidEarning(string reason) =>
            DomainError.Create("Employee.InvalidEarning", $"Earning cannot be applied: {reason}");

        public static DomainError InvalidDeduction(string reason) =>
            DomainError.Create(
                "Employee.InvalidDeduction",
                $"Deduction cannot be applied: {reason}"
            );

        public static DomainError TerminationBeforeHire(EmployeeId id) =>
            DomainError.Create(
                "Employee.TerminationBeforeHire",
                $"Cannot terminate employee {id} before their hire date."
            );

        public static DomainError InvalidSalary(EmployeeId id) =>
            DomainError.Create(
                "Employee.InvalidSalary",
                $"Invalid salary amount for employee {id}."
            );

        public static DomainError DuplicateCompensation(string code) =>
            DomainError.Create(
                "Employee.DuplicateCompensation",
                $"Compensation with code '{code}' already added."
            );

        public static DomainError PolicyAssignmentMismatch(EmployeeId employeeId) =>
            DomainError.Create(
                "Employee.PolicyAssignmentMismatch",
                $"Policy for employee '{employeeId}' has been assigned to another employee."
            );

        public static DomainError PolicyAlreadyAssigned(
            EmployeeId employeeId,
            Guid payrollPolicyId
        ) =>
            DomainError.Create(
                "Employee.PolicyAlreadyAssigned",
                $"Policy with code '{payrollPolicyId}' for employee '{employeeId}' is already added."
            );
    }

    public static class Payroll
    {
        public static DomainError InvalidPeriod(string reason) =>
            DomainError.Create("Payroll.InvalidPeriod", $"Invalid pay period: {reason}");

        public static DomainError CalculationFailed(string employeeName) =>
            DomainError.Create(
                "Payroll.CalculationFailed",
                $"Failed to calculate payslip for {employeeName}"
            );

        public static DomainError InvalidPayPeriod(string reason) =>
            DomainError.Create("Payroll.InvalidPayPeriod", $"Invalid pay period: {reason}");

        public static DomainError PayrollAlreadyProcessed(Guid employeeId) =>
            DomainError.Create(
                "Payroll.InvalidPayPeriod",
                $"Payroll already processed for employee {employeeId}."
            );

        public static DomainError InsufficientSalary =>
            DomainError.Create("Payroll.InsufficientSalary", "Net salary cannot be negative.");

        public static DomainError RunAlreadyCompleted(Guid runId) =>
            DomainError.Create(
                "Payroll.RunAlreadyCompleted",
                $"Payroll run {runId} has already been completed."
            );

        public static DomainError InvalidRunState(Guid runId) =>
            DomainError.Create(
                "Payroll.InvalidRunState",
                $"Payroll run {runId} is in an invalid state for processing."
            );

        public static DomainError DuplicatePayslip(Guid employeeId) =>
            DomainError.Create(
                "Payroll.DuplicatePayslip",
                $"A payslip already exists for employee {employeeId} in this payroll run."
            );
    }

    public static class PayrollPolicy
    {
        public static DomainError InvalidBaseSalary =>
            DomainError.Create(
                "PayrollPolicy.InvalidBaseSalary",
                "Base salary is not a valid amount"
            );

        public static DomainError PolicyNotAssigned(Guid Id) =>
            DomainError.Create(
                "PayrollPolicy.PolicyNotAssigned",
                $"PayrollPolicy with ID {Id} is not assigned."
            );

        public static DomainError PolicyInactive(Guid PolicyId) =>
            DomainError.Create(
                "PayrollPolicy.PolicyInactive",
                $"Policy with ID {PolicyId} is not active."
            );
    }

    public static class Payslip
    {
        public static DomainError PolicyMissing(Guid employeeId, Guid policyId) =>
            DomainError.Create(
                "Payslip.PolicyMissing",
                $"Policy {policyId} is missing for employee {employeeId}."
            );

        public static DomainError PolicyInactive(Guid PolicyId) =>
            DomainError.Create(
                "Payslip.PolicyInactive",
                $"Policy with ID {PolicyId} is not active."
            );

        public static DomainError DuplicatePolicy(Guid policyId) =>
            DomainError.Create("Payslip.DuplicatePolicy", $"Policy '{policyId}' is already added.");
    }
}
