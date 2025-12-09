using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using UsersMembers.Domain.Entities;

namespace UsersMembers.Infrastructure.Services
{
    internal abstract partial class ServiceBase
    {
        protected readonly IMapper Mapper;
        protected readonly ILogger Logger;
        protected readonly IUsuarioRepository UsuarioRepository;
        protected readonly ISessaoUsuarioRepository SessaoUsuarioRepository;
        protected readonly ClaimsUser ClaimsUsuarioLogado;

        protected ServiceBase([NotNull] ServiceBaseDependencies serviceBaseDependencies)
        {
            Mapper = serviceBaseDependencies.Mapper;
            Logger = serviceBaseDependencies.Logger;
            SessaoUsuarioRepository = serviceBaseDependencies.SessionUserRepository;
            UsuarioRepository = serviceBaseDependencies.UserRepository;
            ClaimsUsuarioLogado = ObterUsuarioLogado();
        }

        protected ClaimsUser ObterUsuarioLogado()
        {
            if (UsuarioRepository == null)
                return null;

            return UsuarioRepository.RecuperarUsuarioLogado();
        }
    }
}
