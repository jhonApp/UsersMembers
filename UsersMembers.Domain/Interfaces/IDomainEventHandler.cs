using System.Threading;
using System.Threading.Tasks;

namespace UsersMembers.Domain.Interfaces
{
    public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
    {
        Task Handle(TEvent domainEvent, CancellationToken ct = default);
    }
}
