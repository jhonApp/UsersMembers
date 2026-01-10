using Amazon.DynamoDBv2.DataModel;
using UsersMembers.Application.Interface;
using UsersMembers.Domain.Entities.Audit;
using UsersMembers.Domain.Interfaces;

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
        private readonly IEventProducer _producer;
        private readonly string _queueUrl;

        public AuditEventPublisher(IEventProducer producer)
        {
            _producer = producer;
            // In a real scenario, inject IConfiguration to get this value
            _queueUrl = Environment.GetEnvironmentVariable("AUDIT_QUEUE_URL") ?? "AuditLogQueue";
        }

        public async Task PublishAsync(AuditEvent auditEvent, CancellationToken ct = default)
        {
            // Send the domain event to SQS
            // The Ingestion Lambda will picking it up and save to DynamoDB
            await _producer.PublishAsync(auditEvent, _queueUrl);
        }
    }
}
