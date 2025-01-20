using System.Linq.Expressions;

namespace Kumadio.Core.Common.Interfaces.Base
{
    public interface IRepository<T> where T : class
    {
        // Basic CRUD
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        // "Get all" and "Where" queries
        Task<IList<T>> GetAllAsync();
        Task<T?> GetFirstWhereAsync(Expression<Func<T, bool>> predicate);
        Task<IList<T>> GetAllWhereAsync(Expression<Func<T, bool>> predicate);

        // Save changes or handle transactions
        Task<int> SaveChangesAsync();
    }
}
