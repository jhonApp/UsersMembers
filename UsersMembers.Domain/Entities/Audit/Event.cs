namespace UsersMembers.Domain.Entities.Audit
{
    public class Event
    {
        public Event()
        {
            MetadataEvents = new HashSet<MetadataEvent>();
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime DateHourOcorencia { get; set; }
        public string NameFuncionally { get; set; }
        public Guid IdSection { get; set; }
        public string NameAcessUser { get; set; }
        public short IdTypeEvent { get; set; }
        public Guid IdExecution { get; set; }

        public TypeEvent typeEvent { get; set; }
        public IEnumerable<MetadataEvent> MetadataEvents { get; set; }

    }
}
