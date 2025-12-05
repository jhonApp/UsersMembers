using UsersMembers.Application.ViewModels.User;

namespace UsersMembers.Application.Interface
{
    public interface IUserService
    {
        List<ResponseUser> CreateUsers(RequestUser requestUser);
        List<ResponseUser> UpdateUsers(RequestUser requestUser);
        List<ResponseUser> DeleteUsers(RequestUser requestUser);
    }
}
