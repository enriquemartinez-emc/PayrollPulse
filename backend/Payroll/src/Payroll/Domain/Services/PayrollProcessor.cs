using Payroll.Domain.Common;
using Payroll.Domain.ValueObjects;

namespace Payroll.Domain.Services;

public static class PayrollProcessor
{
    public static Result<PayrollRun> GeneratePayroll(
        IEnumerable<Employee> employees,
        PayPeriod period
    )
    {
        var runResult = PayrollRun.Create(period);
        if (runResult.IsFailure)
            return Result.Failure<PayrollRun>(runResult.Error!);

        var payrollRun = runResult.Value!;

        var payslipResults = employees
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Select(employee => GeneratePayslip(employee, period))
            .ToList();

        var successes = payslipResults.Where(r => r.IsSuccess).Select(r => r.Value!).ToList();

        var failures = payslipResults
            .Where(r => r.IsFailure)
            .Select(r => new PayrollRunFailure
            {
                Id = Guid.NewGuid(),
                PayrollRunId = payrollRun.Id,
                EmployeeId = null,
                Code = r.Error!.Code,
                Message = r.Error!.Message,
            })
            .ToList();

        payrollRun.AddPayslips(successes);
        payrollRun.AddFailures(failures);
        payrollRun.RecalculateTotals();

        return Result.Success(payrollRun);
    }

    private static Result<Payslip> GeneratePayslip(Employee employee, PayPeriod period)
    {
        var payslipResult = Payslip.Create(employee, period);
        if (payslipResult.IsFailure)
            return Result.Failure<Payslip>(payslipResult.Error!);

        var payslip = payslipResult.Value!;

        foreach (
            var assignment in employee.EmployeePayrollPolicies.Where(p =>
                p.IsActiveForPeriod(period)
            )
        )
        {
            var policy = assignment.PayrollPolicy;
            if (policy == null)
                return Result.Failure<Payslip>(
                    Errors.Payslip.PolicyMissing(employee.EmployeeId.Value, assignment.Id)
                );

            var policyCalculationResult = policy.Apply(
                employee.BaseSalary,
                assignment.OverrideRateOrAmount
            );
            if (policyCalculationResult.IsFailure)
                return Result.Failure<Payslip>(policyCalculationResult.Error!);

            var amount = policyCalculationResult.Value!;
            var addPayslipItemResult = payslip.AddItem(
                policy,
                assignment.OverrideRateOrAmount,
                amount
            );
            if (addPayslipItemResult.IsFailure)
                return Result.Failure<Payslip>(addPayslipItemResult.Error!);
        }

        return Result.Success(payslip);
    }
}
