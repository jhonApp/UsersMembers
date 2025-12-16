namespace UsersMembers.Domain.Entities
{
    public class ResultOperation
    {
        public ResultOperation(bool sucess, IEnumerable<MessageOperation> messages)
        {
            Sucess = sucess;
            Message = messages;
        }

        public bool Sucess { get; set; }
        public IEnumerable<MessageOperation> Message { get; set; }


    }
}
