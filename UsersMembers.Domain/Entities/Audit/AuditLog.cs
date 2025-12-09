using Amazon.DynamoDBv2.DataModel;

namespace UsersMembers.Domain.Entities.Audit
{
    [DynamoDBTable("AuditLogs")]
    public class AuditLog
    {
        [DynamoDBHashKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!;
        public string Action { get; set; } = null!;
        public string EntityName { get; set; } = null!;
        public string EntityId { get; set; } = null!;

        public string? DataBefore { get; set; }
        public string? DataAfter { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
    }
}
