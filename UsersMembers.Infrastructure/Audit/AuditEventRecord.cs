using Amazon.DynamoDBv2.DataModel;
using UsersMembers.Application.Interface;
using UsersMembers.Domain.Entities.Audit;

namespace UsersMembers.Infrastructure.Audit
{
    [DynamoDBTable("AuditEvents")]
    public class AuditEventRecord
    {
        [DynamoDBHashKey]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string EventType { get; set; }
        public string UserId { get; set; }
        public string EntityId { get; set; }
        public string EntityType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string CorrelationId { get; set; }

        public string DataJson { get; set; }
    }

    public class AuditEventPublisher : IAuditEventPublisher
    {
        private readonly IDynamoDBContext _context;

        public AuditEventPublisher(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task PublishAsync(AuditEvent auditEvent, CancellationToken ct = default)
        {
            var record = new AuditEventRecord
            {
                EventType    = auditEvent.EventType,
                UserId       = auditEvent.UserId,
                EntityId     = auditEvent.EntityId,
                EntityType   = auditEvent.EntityType,
                Timestamp    = auditEvent.Timestamp,
                Description  = auditEvent.Description,
                CorrelationId = auditEvent.CorrelationId,
                DataJson     = auditEvent.Data != null
                    ? System.Text.Json.JsonSerializer.Serialize(auditEvent.Data)
                    : null
            };

            await _context.SaveAsync(record, ct);
        }
    }
}
