using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Payroll.Infrastructure.Database.Configurations;

public sealed class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
{
    public void Configure(EntityTypeBuilder<ChatConversation> builder)
    {
        builder.ToTable("chat_conversations");
        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.History)
            .HasColumnType("jsonb") // PostgreSQL jsonb
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v =>
                    JsonSerializer.Deserialize<List<ChatTurn>>(v, (JsonSerializerOptions?)null)
                    ?? new()
            );

        builder.HasIndex(x => new { x.UserId, x.PayslipId });
    }
}
