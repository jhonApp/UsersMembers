using System;
using UsersMembers.Domain.Interfaces;

namespace UsersMembers.Domain.Events.UserEvents
{
    public class UserCreatedEvent : IDomainEvent
    {
        public string EventType => "User.Created";
        public string UserId { get; }
        public string NewUserId { get; }
        public string NewUserEmail { get; }
        public DateTime Timestamp { get; }
        public string CorrelationId { get; }

        public UserCreatedEvent(string userId, string newUserId, string newUserEmail, string correlationId)
        {
            UserId = userId;
            NewUserId = newUserId;
            NewUserEmail = newUserEmail;
            CorrelationId = correlationId;
            Timestamp = DateTime.UtcNow;
        }
    }
}
