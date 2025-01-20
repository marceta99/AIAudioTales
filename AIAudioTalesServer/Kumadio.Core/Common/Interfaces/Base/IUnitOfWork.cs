using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kumadio.Core.Common.Interfaces.Base
{
    public interface IUnitOfWork : IDisposable
    {
        // A method returning Result (no data), 
        Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action);

        // A method returning Result<T> if your callback 
        Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> action);
    }
}
