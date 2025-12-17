using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.DependencyInjection;
using UsersMembers.Application.Interface;
using UsersMembers.Application.Service.Users;
using UsersMembers.Application.EventHandlers;
using UsersMembers.Infrastructure.Audit;
using UsersMembers.Infrastructure.Events;
using UsersMembers.Domain.Interfaces;
using UsersMembers.Domain.Events.UserEvents;

namespace UsersMembers.Ioc
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUsersMembersDependencies(
            this IServiceCollection services,
            string awsRegion)
        {
            services.AddSingleton<IAmazonDynamoDB>(sp =>
               new AmazonDynamoDBClient(Amazon.RegionEndpoint.GetBySystemName(awsRegion)));

            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            services.AddScoped<IUserService, UsersService>();
            services.AddSingleton<IAuditEventPublisher, AuditEventPublisher>();

            // Domain Events
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
            services.AddScoped<IDomainEventHandler<UserCreatedEvent>, AuditEventHandler>();

            return services;
        }
    }
}
