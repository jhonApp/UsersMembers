using Bogus;
using UsersMembers.Domain.Entities;

namespace UsersMembers.Tests.Builders
{
    public class UserBuilder
    {
        private readonly Faker _faker;
        private string _name;
        private string _email;
        private int _typeUser;

        public UserBuilder()
        {
            _faker = new Faker("pt_BR");
            _name = _faker.Name.FullName();
            _email = _faker.Internet.Email();
            _typeUser = 1;
        }

        public UserBuilder WithName(string name)
        {
            _name = name;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserBuilder WithInvalidEmail()
        {
            _email = "invalid-email";
            return this;
        }

        public UserBuilder WithTypeUser(int typeUser)
        {
            _typeUser = typeUser;
            return this;
        }

        public User Build()
        {
            return new User(_name, _email, _typeUser);
        }
    }
}
