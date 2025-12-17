using System.Threading;
using System.Threading.Tasks;
using UsersMembers.Application.Interface;
using UsersMembers.Domain.Entities.Audit;
using UsersMembers.Domain.Events.UserEvents;
using UsersMembers.Domain.Interfaces;

namespace UsersMembers.Application.EventHandlers
{
    public class AuditEventHandler : IDomainEventHandler<UserCreatedEvent>
    {
        private readonly IAuditEventPublisher _auditPublisher;

        public AuditEventHandler(IAuditEventPublisher auditPublisher)
        {
            _auditPublisher = auditPublisher;
        }

        public async Task Handle(UserCreatedEvent domainEvent, CancellationToken ct = default)
        {
            var auditEvent = new AuditEvent
            {
                EventType = domainEvent.EventType,
                UserId = domainEvent.UserId,
                EntityId = domainEvent.NewUserId,
                EntityType = "User",
                Description = "User created",
                Timestamp = domainEvent.Timestamp,
                Data = new { domainEvent.NewUserId, domainEvent.NewUserEmail },
                CorrelationId = domainEvent.CorrelationId
            };

            await _auditPublisher.PublishAsync(auditEvent, ct);
        }
    }
}
