namespace UsersMembers.Domain.Entities.Audit
{
    public class AuditEvent
    {
        public string EventType { get; set; }
        public string UserId { get; set; }
        public string EntityId { get; set; } 
        public string EntityType { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Description { get; set; }
        public object Data { get; set; }
        public string CorrelationId { get; set; }
    }
}
