namespace Payroll.Features.Employees;

public sealed record EmployeeResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    decimal BaseSalary,
    string Department,
    DateOnly HireDate
);
