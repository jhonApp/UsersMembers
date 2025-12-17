using UsersMembers.Domain.Entities;
using UsersMembers.Domain.Interface;

namespace UsersMembers.Domain.Interfaces
{
    public interface IUsersRepository : IBaseRepository<User>
    {
        new Task CreateAsync(User user, CancellationToken ct);
    }
}
