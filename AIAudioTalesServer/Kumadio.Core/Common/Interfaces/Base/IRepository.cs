using System.Linq.Expressions;

namespace Kumadio.Core.Common.Interfaces.Base
{
    public interface IRepository<T> where T : class
    {
        // Basic CRUD
        Task<T?> GetById(int id);
        Task Add(T entity);
        Task AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> Any(Expression<Func<T, bool>> predicate);

        // "Get all" and "Where" queries
        Task<IList<T>> GetAll();
        Task<T?> GetFirstWhere(Expression<Func<T, bool>> predicate);
        Task<IList<T>> GetAllWhere(Expression<Func<T, bool>> predicate);

        // Save changes or handle transactions
        Task<int> SaveChanges();
    }
}
