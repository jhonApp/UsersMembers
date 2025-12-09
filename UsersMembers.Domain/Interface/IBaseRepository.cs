using UsersMembers.Domain.Entities;

namespace UsersMembers.Domain.Interface
{
    public interface IBaseRepository<TEntity> where TEntity: class
    {
        public ResultOperation Create(TEntity entity);
        public TEntity GetById(int id);
        public ResultOperation Update(TEntity entity);
        public ResultOperation Delete(TEntity entity);
    }
}
