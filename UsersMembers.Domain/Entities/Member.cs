namespace UsersMembers.Domain.Entities
{
    public class Member
    {
        public int ChurchId { get; set; }
        public string? Status { get; set; }
        public DateTime EntryDate { get; set; }
        public DateTime BaptismDate { get; set; }
    }
}
