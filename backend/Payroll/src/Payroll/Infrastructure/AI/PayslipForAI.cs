namespace Payroll.Infrastructure.AI;

public sealed class PayslipForAI
{
    public Guid Id { get; set; }
    public Guid EmployeeId { get; set; }
    public decimal NetPay { get; set; }
    public decimal TotalBonuses { get; set; }
    public decimal TotalDeductions { get; set; }
    public List<ItemForAI> Items { get; set; } = [];
}

public sealed class ItemForAI
{
    public string Name { get; set; } = default!;
    public decimal Amount { get; set; }
    public string PolicyName { get; set; } = default!;
    public string PolicyDescription { get; set; } = default!;
    public string PolicyCalculationType { get; set; } = default!;
    public string PolicyRateOrAmount { get; set; } = default!;
}
