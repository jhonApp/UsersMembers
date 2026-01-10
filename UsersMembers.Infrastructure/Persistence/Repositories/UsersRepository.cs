using Amazon.DynamoDBv2.DataModel;
using UsersMembers.Domain.Entities;
using UsersMembers.Domain.Interfaces;
using UsersMembers.Domain.Interface;

namespace UsersMembers.Infrastructure.Persistence.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly IDynamoDBContext _context;

        public UsersRepository(IDynamoDBContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(User user, CancellationToken ct)
        {
            await _context.SaveAsync(user, ct);
        }

        // Explicit implementation for IBaseRepository<User> to handle the return type conflict
        async Task<ResultOperation> IBaseRepository<User>.CreateAsync(User entity, CancellationToken ct)
        {
            await _context.SaveAsync(entity, ct);
            return new ResultOperation { Success = true };
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _context.LoadAsync<User>(id);
        }

        public Task<ResultOperation> UpdateAsync(User entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<ResultOperation> DeleteAsync(User entity, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
