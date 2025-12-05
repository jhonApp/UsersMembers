using Microsoft.Extensions.DependencyInjection;
using UsersMembers.Application.Interface;
using UsersMembers.Application.Service.Users;

namespace UsersMembers.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsersMembersDependencies(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UsersService>();

            return services;
        }
    }
}
