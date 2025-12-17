namespace UsersMembers.Domain.Entities
{
    public class ResultOperation
    {
        public ResultOperation() { }

        public ResultOperation(bool success, IEnumerable<MessageOperation> messages)
        {
            Success = success;
            Message = messages;
        }

        public bool Success { get; set; }
        public IEnumerable<MessageOperation> Message { get; set; }
        public object Data { get; set; }
    }
}
