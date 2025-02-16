namespace Kumadio.Core.Common.Interfaces.Base
{
    public interface IUnitOfWork : IDisposable
    {
        // A method returning Result (no data), 
        Task<Result> ExecuteInTransaction(Func<Task<Result>> action);

        // A method returning Result<T> if your callback 
        Task<Result<T>> ExecuteInTransaction<T>(Func<Task<Result<T>>> action);
    }
}
