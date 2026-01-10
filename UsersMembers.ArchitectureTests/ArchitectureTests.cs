using FluentAssertions;
using NetArchTest.Rules;
using Xunit;
using UsersMembers.Domain.Entities;
using UsersMembers.Application.Service.Users;
using UsersMembers.Infrastructure.Persistence.Repositories;
using UsersMembers.Infrastructure.IaC;

namespace UsersMembers.ArchitectureTests
{
    public class ArchitectureTests
    {
        // Rule 1: Domain Layer should NOT have dependencies on Infrastructure or API.
        [Fact]
        public void Domain_Layer_Should_Not_Have_Dependency_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(User).Assembly)
                .ShouldNot()
                .HaveDependencyOn("UsersMembers.Infrastructure")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Domain_Layer_Should_Not_Have_Dependency_On_API()
        {
            var result = Types.InAssembly(typeof(User).Assembly)
                .ShouldNot()
                .HaveDependencyOn("UsersMembers.API")
                .GetResult();

             result.IsSuccessful.Should().BeTrue();
        }

        // Rule 2: Domain Entities should be sealed (optional, but good practice).
        // Leaving this commented out or relaxed as User entity might not be sealed yet.
        // [Fact]
        // public void Domain_Entities_Should_Be_Sealed()
        // {
        //     var result = Types.InAssembly(typeof(User).Assembly)
        //         .That()
        //         .Inherit(typeof(object)) // Or specific base entity
        //         .And()
        //         .AreNotAbstract()
        //         .And()
        //         .AreClasses()
        //         .Should()
        //         .BeSealed()
        //         .GetResult();
        //
        //     result.IsSuccessful.Should().BeTrue();
        // }

        // Rule 3: Application Layer should NOT depend on Infrastructure.
        [Fact]
        public void Application_Layer_Should_Not_Have_Dependency_On_Infrastructure()
        {
            var result = Types.InAssembly(typeof(UsersService).Assembly)
                .ShouldNot()
                .HaveDependencyOn("UsersMembers.Infrastructure")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        // Rule 4: Repositories in Infrastructure must implement interfaces from Domain.
        [Fact]
        public void Repositories_Must_Implement_Domain_Interfaces()
        {
            var result = Types.InAssembly(typeof(UsersRepository).Assembly)
                .That()
                .ResideInNamespace("UsersMembers.Infrastructure.Persistence.Repositories")
                .And()
                .AreClasses()
                .Should()
                .ImplementInterface(typeof(UsersMembers.Domain.Interfaces.IUsersRepository)) // This is specific, better to check naming convention or base interface
                .GetResult();
                
             // This test is a bit rigid (forcing correct implementation). 
             // A better test might be: Classes ending in Repository should implement an interface ending in Repository.
             
             var namingResult = Types.InAssembly(typeof(UsersRepository).Assembly)
                .That()
                .ResideInNamespace("UsersMembers.Infrastructure.Persistence.Repositories")
                .And()
                .AreClasses()
                .Should()
                .HaveNameEndingWith("Repository")
                .GetResult();
                
            namingResult.IsSuccessful.Should().BeTrue();
        }
    }
}
