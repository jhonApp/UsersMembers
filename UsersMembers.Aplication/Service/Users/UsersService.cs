using UsersMembers.Application.Interface;
using UsersMembers.Application.ViewModels.User;
using UsersMembers.Domain.Entities;
using UsersMembers.Domain.Entities.Audit;

namespace UsersMembers.Application.Service.Users
{
    public class UsersService : IUserService
    {
        private readonly IAuditEventPublisher _auditPublisher;
        private readonly ICurrentUserService _currentUser;
        private readonly IUsersRepository _usersRepository;

        public UsersService(
        IAuditEventPublisher auditPublisher,
        ICurrentUserService currentUser,
        IUsersRepository usersRepository)
        {
            _auditPublisher = auditPublisher;
            _currentUser = currentUser;
            _usersRepository = usersRepository;
        }

        public ResultOperation Create(RequestUser entity)
        {
            await _usersRepository.CreateAsync(newUser, ct);
            var response = new List<ResponseUser> { MapToResponse(newUser) };

            var auditEvent = new AuditEvent
            {
                EventType   = "User.Created",
                UserId      = _currentUser.UserId,
                EntityId    = newUser.Id,
                EntityType  = "User",
                Description = "User created",
                Data        = new { newUser.Id, newUser.Email },
                CorrelationId = _currentUser.CorrelationId
            };

            await _auditPublisher.PublishAsync(auditEvent, ct);
            return response;
        }

        public ResultOperation Delete(RequestUser entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<ResponseUser>> GetByCPF(RequestUser requestUser)
        {
            throw new NotImplementedException();
        }

        public RequestUser GetById(int id)
        {
            throw new NotImplementedException();
        }

        public ResultOperation Update(RequestUser entity)
        {
            throw new NotImplementedException();
        }
    }
}
