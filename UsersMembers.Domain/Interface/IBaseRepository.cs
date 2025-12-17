using UsersMembers.Domain.Entities;

namespace UsersMembers.Domain.Interface
{
    public interface IBaseRepository<TEntity> where TEntity: class
    {
        public Task<ResultOperation> CreateAsync(TEntity entity, CancellationToken ct = default);
        public Task<TEntity> GetByIdAsync(string id);
        public Task<ResultOperation> UpdateAsync(TEntity entity, CancellationToken ct = default);
        public Task<ResultOperation> DeleteAsync(TEntity entity, CancellationToken ct = default);
    }
}
