using System.Threading.Tasks;

namespace UsersMembers.Domain.Interfaces
{
    public interface IEventProducer
    {
        Task PublishAsync<T>(T @event, string queueUrl);
    }
}
