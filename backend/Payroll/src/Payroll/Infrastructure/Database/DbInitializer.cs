using Payroll.Domain.Services;

namespace Payroll.Infrastructure.Database;

public static class DbInitializer
{
    public static async Task InitializeAsync(
        PayrollDbContext context,
        CancellationToken ct = default
    )
    {
        await context.Database.MigrateAsync(ct);

        if (await context.Departments.AnyAsync(ct))
            return;

        // Departments
        var hr = new Department("Human Resources");
        var dev = new Department("Development");
        var finance = new Department("Finance");
        var ops = new Department("Operations");
        context.Departments.AddRange(hr, dev, finance, ops);

        // Policies
        var baseSalary = PayrollPolicy.Create(
            "Base Salary",
            "Fixed base salary",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            0m
        );
        var perfBonus = PayrollPolicy.Create(
            "Performance Bonus",
            "Performance (10%)",
            CompensationType.Earning,
            CalculationType.PercentageOfBase,
            10m
        );
        var overtime = PayrollPolicy.Create(
            "Overtime",
            "Overtime fixed",
            CompensationType.Earning,
            CalculationType.FixedAmount,
            150m
        );
        var incomeTax = PayrollPolicy.Create(
            "Income Tax",
            "Income tax 10%",
            CompensationType.Deduction,
            CalculationType.PercentageOfBase,
            10m
        );
        var pension = PayrollPolicy.Create(
            "Pension",
            "Pension 5%",
            CompensationType.Deduction,
            CalculationType.PercentageOfBase,
            5m
        );
        var health = PayrollPolicy.Create(
            "Health Insurance",
            "Health fixed",
            CompensationType.Deduction,
            CalculationType.FixedAmount,
            100m
        );
        var late = PayrollPolicy.Create(
            "Late Penalty",
            "Late penalty",
            CompensationType.Deduction,
            CalculationType.FixedAmount,
            50m
        );

        context.PayrollPolicies.AddRange(
            baseSalary,
            perfBonus,
            overtime,
            incomeTax,
            pension,
            health,
            late
        );
        await context.SaveChangesAsync(ct);

        // Employees
        var john = Employee
            .Create(
                EmployeeId.New(),
                "John",
                "Doe",
                "john.doe@company.com",
                new Money(2000m),
                hr,
                new DateOnly(2023, 1, 10)
            )
            .Value!;

        var jane = Employee
            .Create(
                EmployeeId.New(),
                "Jane",
                "Smith",
                "jane.smith@company.com",
                new Money(2500m),
                dev,
                new DateOnly(2022, 5, 1)
            )
            .Value!;

        var alice = Employee
            .Create(
                EmployeeId.New(),
                "Alice",
                "Johnson",
                "alice.johnson@company.com",
                new Money(3000m),
                finance,
                new DateOnly(2021, 11, 20)
            )
            .Value!;

        var bob = Employee
            .Create(
                EmployeeId.New(),
                "Bob",
                "Williams",
                "bob.williams@company.com",
                new Money(1800m),
                ops,
                new DateOnly(2023, 3, 1)
            )
            .Value!;

        context.Employees.AddRange(john, jane, alice, bob);
        await context.SaveChangesAsync(ct);

        // Assign policies to employees (employee-specific overrides shown)
        var assignments = new List<EmployeePayrollPolicy>
        {
            EmployeePayrollPolicy.Assign(john.EmployeeId, baseSalary),
            EmployeePayrollPolicy.Assign(john.EmployeeId, incomeTax),
            EmployeePayrollPolicy.Assign(john.EmployeeId, pension),
            EmployeePayrollPolicy.Assign(john.EmployeeId, health),
            EmployeePayrollPolicy.Assign(jane.EmployeeId, baseSalary),
            EmployeePayrollPolicy.Assign(jane.EmployeeId, perfBonus),
            EmployeePayrollPolicy.Assign(jane.EmployeeId, incomeTax),
            EmployeePayrollPolicy.Assign(jane.EmployeeId, pension),
            EmployeePayrollPolicy.Assign(jane.EmployeeId, health),
            EmployeePayrollPolicy.Assign(alice.EmployeeId, baseSalary),
            EmployeePayrollPolicy.Assign(alice.EmployeeId, perfBonus),
            EmployeePayrollPolicy.Assign(alice.EmployeeId, overtime),
            EmployeePayrollPolicy.Assign(alice.EmployeeId, incomeTax),
            EmployeePayrollPolicy.Assign(alice.EmployeeId, health),
            EmployeePayrollPolicy.Assign(bob.EmployeeId, baseSalary),
            EmployeePayrollPolicy.Assign(bob.EmployeeId, incomeTax),
            EmployeePayrollPolicy.Assign(bob.EmployeeId, pension),
            EmployeePayrollPolicy.Assign(bob.EmployeeId, late),
        };

        await context.Set<EmployeePayrollPolicy>().AddRangeAsync(assignments, ct);

        // Also adding to each employee aggregate so EF tracking + domain state align
        john.AssignPolicy(assignments[0]);
        john.AssignPolicy(assignments[1]);
        john.AssignPolicy(assignments[2]);
        john.AssignPolicy(assignments[3]);

        jane.AssignPolicy(assignments[4]);
        jane.AssignPolicy(assignments[5]);
        jane.AssignPolicy(assignments[6]);
        jane.AssignPolicy(assignments[7]);
        jane.AssignPolicy(assignments[8]);

        alice.AssignPolicy(assignments[9]);
        alice.AssignPolicy(assignments[10]);
        alice.AssignPolicy(assignments[11]);
        alice.AssignPolicy(assignments[12]);
        alice.AssignPolicy(assignments[13]);

        bob.AssignPolicy(assignments[14]);
        bob.AssignPolicy(assignments[15]);
        bob.AssignPolicy(assignments[16]);
        bob.AssignPolicy(assignments[17]);

        await context.SaveChangesAsync(ct);

        // Generate payroll runs (4 months sample)
        var employees = new[] { john, jane, alice, bob };
        for (int month = 7; month <= 10; month++)
        {
            var period = new PayPeriod(new DateOnly(2024, month, 1), new DateOnly(2024, month, 30));
            var result = PayrollProcessor.GeneratePayroll(employees, period);
            if (result.IsSuccess)
                context.PayrollRuns.Add(result.Value!);
        }

        if (!await context.Users.AnyAsync(ct))
        {
            const string adminEmail = "admin@company.com";
            const string adminPassword = "Admin1234!";

            var (hash, salt) = PasswordHasher.HashPassword(adminPassword);
            var adminUser = User.CreateStandalone(adminEmail, hash, salt, Roles.Admin);

            context.Users.Add(adminUser);

            const string bobPassword = "Bob1234!";

            var (hashBob, saltBob) = PasswordHasher.HashPassword(bobPassword);

            var employeeUser = User.CreateForEmployee(
                bob.EmployeeId,
                hashBob,
                saltBob,
                Roles.Employee
            );

            employeeUser.SetEmail("bob.williams@company.com");

            context.Users.Add(employeeUser);
        }

        await context.SaveChangesAsync(ct);
    }
}
