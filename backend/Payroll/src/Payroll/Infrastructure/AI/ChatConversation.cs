namespace Payroll.Infrastructure.AI;

public sealed class ChatConversation
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid PayslipId { get; set; }

    public List<ChatTurn> History { get; private set; } = [];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public ChatConversation() { }

    public void AddUserMessage(string message) => History.Add(new ChatTurn("user", message));

    public void AddAssistantMessage(string message) =>
        History.Add(new ChatTurn("assistant", message));

    public void AddSystemMessage(string message) => History.Add(new ChatTurn("system", message));
}

public sealed record ChatTurn(string Role, string Content);
