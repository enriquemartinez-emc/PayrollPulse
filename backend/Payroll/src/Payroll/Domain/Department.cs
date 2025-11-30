namespace Payroll.Domain;

public sealed class Department(string name)
{
    private readonly List<Employee> _employees = [];
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; } = name;

    public IReadOnlyList<Employee> Employees => _employees.AsReadOnly();

    public void AddEmployee(Employee employee) => _employees.Add(employee);

    public void RemoveEmployee(Employee employee) => _employees.Remove(employee);
}
