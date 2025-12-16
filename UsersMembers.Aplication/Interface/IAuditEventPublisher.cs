using UsersMembers.Domain.Entities.Audit;

namespace UsersMembers.Application.Interface
{
    public interface IAuditEventPublisher
    {
        Task PublishAsync(AuditEvent auditEvent, CancellationToken ct = default);
    }
}
