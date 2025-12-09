namespace UsersMembers.Domain.Entities.Audit
{
    public class TypeEvent
    {
        public short CodTypeEvent { get; set; }
        public string Description { get; set; }
        public IEnumerable<Event> Events { get; set; }
    }
}
