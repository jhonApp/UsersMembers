using UsersMembers.Application.ViewModels.User;
using UsersMembers.Domain.Interface;

namespace UsersMembers.Application.Interface
{
    public interface IUserService : IBaseRepository<RequestUser>
    {
        Task<List<ResponseUser>> GetByCPF(RequestUser requestUser);
    }
}
