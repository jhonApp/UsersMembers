namespace UsersMembers.Domain.Entities.Audit
{
    public class MetadataEvent
    {
        public Guid Id { get; set; }
        public Guid IdEvent { get; set; }
        public DateTime DateHourMetadata { get; set; }
        public string NameKey { get; set; }
        public string ValueMetadata { get; set; }
        public Event Event { get; set; }
    }
}
