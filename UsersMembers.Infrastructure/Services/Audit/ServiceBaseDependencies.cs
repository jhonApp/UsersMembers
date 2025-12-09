using AutoMapper;
using Microsoft.Extensions.Logging;

namespace UsersMembers.Infrastructure.Services
{
    internal class ServiceBaseDependencies
    {
        public ServiceBaseDependencies(IMapper mapper, ILogger logger, IUserRepository userRepository, ISessionUserRepository sessionUserRepository)
        {
            Mapper = mapper;
            Logger = logger;
            UserRepository = userRepository;
            SessionUserRepository = sessionUserRepository;
        }

        public IMapper Mapper { get; }
        public ILogger Logger { get; }
        public IUserRepository UserRepository { get; }
        public ISessionUserRepository SessionUserRepository { get; }
    }
}
