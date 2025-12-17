using UsersMembers.Application.Interface;
using UsersMembers.Domain.Entities;
using UsersMembers.Domain.Entities.Audit;

namespace UsersMembers.Application.Extensions
{
    public static class AuditExtensions
    {
        public static async Task PublishUserCreatedAsync(
            this IAuditEventPublisher publisher,
            User user,
            CancellationToken ct = default)
        {
            var auditEvent = new AuditEvent
            {
                EventType = "User.Created",
                UserId = Convert.ToString(user.Id),
                EntityId = user.Id.ToString(),
                EntityType = "User",
                Description = $"User {user.Name} created successfully",
                Data = new { user.Id, user.Email, user.Name },
                CorrelationId = Convert.ToString(user.Id),
                Timestamp = DateTime.UtcNow
            };

            await publisher.PublishAsync(auditEvent, ct);
        }
    }
}
