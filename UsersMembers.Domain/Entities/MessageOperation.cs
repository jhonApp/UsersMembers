namespace UsersMembers.Domain.Entities
{
    public abstract class MessageOperation
    {
        public string Cod { get; set; }
        public string Description { get; set; }

        protected MessageOperation(string cod, string description)
        {
            Cod=cod;
            Description=description;
        }
    }
}
