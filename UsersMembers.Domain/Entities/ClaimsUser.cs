namespace UsersMembers.Domain.Entities
{
    public class ClaimsUser
    {
        public string? IdUser { get; set; }
        public string? NameAccess { get; set; }
        public int NumberAgency { get; set; }
        public string? CPF { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? Token { get; set; }
        public string? NameUser { get; set; }
        public string? Profile { get; set; }
        public Guid IdSession { get; set; }
    }
}
