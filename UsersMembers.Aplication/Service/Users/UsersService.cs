using UsersMembers.Application.Interface;
using UsersMembers.Application.ViewModels.User;

namespace UsersMembers.Application.Service.Users
{
    public class UsersService : IUserService
    {
        private readonly IAuditService _auditService;

        public UsersService(IAuditService auditService)
        {
            _auditService = auditService;
        }

        public async Task<List<ResponseUser>> CreateUsers(RequestUser requestUser)
        {
            var newUserId = Guid.NewGuid().ToString();

            return await _auditService.ExecuteWithAuditAsync(
                action: "CreateUser",
                entityName: "User",
                entityId: newUserId,
                func: async () =>
                {
                    var newUser = new ResponseUser
                    {
                        Id = newUserId,
                        Name = requestUser.Name
                    };

                    // salvar usuário no DynamoDB aqui
                    // await _userRepository.CreateAsync(newUser);

                    return new List<ResponseUser> { newUser };
                });
        }

        public List<ResponseUser> DeleteUsers(RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        public List<ResponseUser> UpdateUsers(RequestUser requestUser)
        {
            throw new NotImplementedException();
        }
    }
}
