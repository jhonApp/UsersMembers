namespace UsersMembers.Application.Interface
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        string CorrelationId { get; }
    }
}
