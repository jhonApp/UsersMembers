using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using UsersMembers.Application.EventHandlers;
using UsersMembers.Application.Interface;
using UsersMembers.Domain.Entities.Audit;
using UsersMembers.Domain.Events.UserEvents;
using Xunit;

namespace UsersMembers.UnitTests
{
    public class AuditEventHandlerTests
    {
        private readonly Mock<IAuditEventPublisher> _publisherMock;
        private readonly AuditEventHandler _handler;

        public AuditEventHandlerTests()
        {
            _publisherMock = new Mock<IAuditEventPublisher>();
            _handler = new AuditEventHandler(_publisherMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldPublishAuditEvent_WhenEventIsValid()
        {
            // Arrange
            var userId = "admin_123";
            var newUserId = "user_456";
            var newUserEmail = "test@example.com";
            var correlationId = "corr_789";
            var userEvent = new UserCreatedEvent(userId, newUserId, newUserEmail, correlationId);

            // Act
            await _handler.Handle(userEvent, CancellationToken.None);

            // Assert
            _publisherMock.Verify(p => p.PublishAsync(It.Is<AuditEvent>(e =>
                e.EventType == "User.Created" &&
                e.UserId == userId &&
                e.EntityId == newUserId &&
                e.EntityType == "User" &&
                e.CorrelationId == correlationId &&
                e.Description == "User created"
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldMapDataCorrectly()
        {
            // Arrange
            var userEvent = new UserCreatedEvent("u1", "u2", "e@mail.com", "c1");

            // Act
            await _handler.Handle(userEvent);

            // Assert
            _publisherMock.Verify(p => p.PublishAsync(It.Is<AuditEvent>(e =>
                e.Data != null &&
                CheckAnonymousObject(e.Data, "u2", "e@mail.com")
            ), It.IsAny<CancellationToken>()), Times.Once);
        }

        private bool CheckAnonymousObject(object actualData, string expectedId, string expectedEmail)
        {
             var type = actualData.GetType();
             var pId = type.GetProperty("NewUserId");
             var pEmail = type.GetProperty("NewUserEmail");
             
             if (pId == null || pEmail == null) return false;
             
             var id = pId.GetValue(actualData)?.ToString();
             var email = pEmail.GetValue(actualData)?.ToString();
             
             return id == expectedId && email == expectedEmail;
        }

        [Fact]
        public async Task Handle_ShouldPropagateException_WhenPublisherFails()
        {
            // Arrange
            var userEvent = new UserCreatedEvent("u1", "u2", "e@mail.com", "c1");
            var expectedException = new InvalidOperationException("DB Connection Failed");
            
            _publisherMock.Setup(p => p.PublishAsync(It.IsAny<AuditEvent>(), It.IsAny<CancellationToken>()))
                          .ThrowsAsync(expectedException);

            // Act
            Func<Task> act = async () => await _handler.Handle(userEvent);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("DB Connection Failed");
        }

        [Fact]
        public async Task Handle_ShouldHandleNullCorrelationId_Gracefully()
        {
             // Arrange
            var userEvent = new UserCreatedEvent("u1", "u2", "e@mail.com", null);

            // Act
            await _handler.Handle(userEvent);

            // Assert
            _publisherMock.Verify(p => p.PublishAsync(It.Is<AuditEvent>(e =>
                e.CorrelationId == null
            ), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
