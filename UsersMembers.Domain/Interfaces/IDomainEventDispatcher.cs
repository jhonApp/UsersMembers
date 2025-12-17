using System.Threading;
using System.Threading.Tasks;

namespace UsersMembers.Domain.Interfaces
{
    public interface IDomainEventDispatcher
    {
        Task Dispatch<TEvent>(TEvent domainEvent, CancellationToken ct = default) where TEvent : IDomainEvent;
    }
}
