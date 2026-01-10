using FluentAssertions;
using UsersMembers.Tests.Builders;
using Xunit;

namespace UsersMembers.Tests.Domain
{
    public class UserTests
    {
        [Fact]
        public void Constructor_Should_Create_User_When_Data_Is_Valid()
        {
            // Arrange
            var builder = new UserBuilder();

            // Act
            var user = builder.Build();

            // Assert
            user.Should().NotBeNull();
            user.Id.Should().NotBeNullOrEmpty();
            user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            user.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Constructor_Should_Throw_Exception_When_Name_Is_Empty()
        {
            // Arrange
            var builder = new UserBuilder().WithName("");

            // Act
            Action act = () => builder.Build();

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Name cannot be empty*");
        }

        [Fact]
        public void Constructor_Should_Throw_Exception_When_Email_Is_Invalid()
        {
            // Arrange
            var builder = new UserBuilder().WithInvalidEmail();

            // Act
            Action act = () => builder.Build();

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("Invalid email format*");
        }
    }
}
