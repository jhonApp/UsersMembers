namespace UsersMembers.Domain.Entities
{
    public class Profile
    {
        public string? Type { get; set; }
        public List<string>? Permisions { get; set; }
        public List<string>? Roles { get; set; }
    }
}
