namespace UsersMembers.Domain.Entities
{
    public class History
    {
        public string? Type { get; set; }
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string? ResponsiblePastor { get; set; }
    }
}
