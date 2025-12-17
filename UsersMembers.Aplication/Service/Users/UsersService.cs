using UsersMembers.Application.Interface;
using UsersMembers.Application.ViewModels.User;
using UsersMembers.Domain.Entities;
using UsersMembers.Domain.Events.UserEvents;
using UsersMembers.Domain.Interfaces;

namespace UsersMembers.Application.Service.Users
{
    public class UsersService : IUserService
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ICurrentUserService _currentUser;
        private readonly IUsersRepository _usersRepository;

        public UsersService(
        IDomainEventDispatcher dispatcher,
        ICurrentUserService currentUser,
        IUsersRepository usersRepository)
        {
            _dispatcher = dispatcher;
            _currentUser = currentUser;
            _usersRepository = usersRepository;
        }

        public async Task<ResultOperation> CreateAsync(RequestUser entity, CancellationToken ct = default)
        {
            var newUser = new User 
            {
               Id = Guid.NewGuid().ToString(),
               Email = entity.Email
            };

            await _usersRepository.CreateAsync(newUser, ct);
            
            var userCreatedEvent = new UserCreatedEvent(
                _currentUser.UserId,
                newUser.Id,
                newUser.Email,
                _currentUser.CorrelationId
            );

            await _dispatcher.Dispatch(userCreatedEvent, ct);

            var response = new ResponseUser { Id = newUser.Id, Email = newUser.Email };
            return new ResultOperation { Success = true, Data = response }; 
        }

        public Task<ResultOperation> DeleteAsync(RequestUser entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResponseUser>> GetByCPF(RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        public Task<RequestUser> GetByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<ResultOperation> UpdateAsync(RequestUser entity, CancellationToken ct = default)
        {
             throw new NotImplementedException();
        }
    }
}
