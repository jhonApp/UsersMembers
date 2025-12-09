using AutoMapper;
using Microsoft.Extensions.Logging;
using UsersMembers.Infrastructure.Services;

namespace UsersMembers.Application.Service.Audit
{
    internal class ServiceAuditDependence : ServiceBaseDependencies
    {
        public ServiceAuditDependence(
            IMapper mapper, 
            ILogger logger, 
            IUserRepository userRepository, 
            IEventRepository eventRepository, 
            ISessionUserRepository sessionUserRepository
            ) : base(mapper, logger, userRepository, sessionUserRepository)
        {
            EventRepository = eventRepository;
        }

        public IEventRepository EventRepository { get; }
    }
}
