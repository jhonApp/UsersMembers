namespace UsersMembers.Domain.Entities
{
    public class User
    {
        public string Id { get; set; }
        public int ChurchId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public Profile Profile { get; set; }
        public Address Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastSeenAt { get; set; }
        public bool IsActive { get; set; }
    }
}
