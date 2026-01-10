using FluentAssertions;
using NSubstitute;
using UsersMembers.Application.Interface;
using UsersMembers.Application.Service.Users;
using UsersMembers.Application.ViewModels.User;
using UsersMembers.Domain.Entities;
using UsersMembers.Domain.Events.UserEvents;
using UsersMembers.Domain.Interfaces;
using Xunit;

namespace UsersMembers.Tests.Application
{
    public class UsersServiceTests
    {
        private readonly IDomainEventDispatcher _dispatcher;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUsersRepository _repository;
        private readonly UsersService _service;

        public UsersServiceTests()
        {
            _dispatcher = Substitute.For<IDomainEventDispatcher>();
            _currentUserService = Substitute.For<ICurrentUserService>();
            _repository = Substitute.For<IUsersRepository>();

            _service = new UsersService(_dispatcher, _currentUserService, _repository);
        }

        [Fact]
        public async Task CreateAsync_Should_Create_User_And_Dispatch_Event()
        {
            // Arrange
            var request = new RequestUser { Email = "test@example.com" };
            
            _currentUserService.UserId.Returns("admin-id");
            _currentUserService.CorrelationId.Returns("correlation-123");

            _repository.CreateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(new ResultOperation { Success = true }));

            // Act
            var result = await _service.CreateAsync(request);

            // Assert
            result.Success.Should().BeTrue();
            
            // Verify Repository was called with correct Email
            await _repository.Received(1).CreateAsync(
                Arg.Is<User>(u => u.Email == request.Email), 
                Arg.Any<CancellationToken>());

            // Verify Event was Dispatched
            await _dispatcher.Received(1).Dispatch(
                Arg.Is<UserCreatedEvent>(e => 
                    e.NewUserEmail == request.Email && 
                    e.UserId == "admin-id" &&
                    e.CorrelationId == "correlation-123"), 
                Arg.Any<CancellationToken>());
        }
    }
}
