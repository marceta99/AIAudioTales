using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Infrastructure.Data;

namespace Kumadio.Infrastructure.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result> ExecuteInTransaction(Func<Task<Result>> action)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await action();
                if(result.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return DomainErrors.Transaction.Failed(ex.Message);               
            }
        }

        public async Task<Result<T>> ExecuteInTransaction<T>(Func<Task<Result<T>>> action)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var result = await action();
                if(result.IsFailure)
                {
                    await transaction.RollbackAsync();
                    return result;
                }

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return DomainErrors.Transaction.Failed(ex.Message);
            }
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
