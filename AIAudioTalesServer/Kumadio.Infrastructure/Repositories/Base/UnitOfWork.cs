using Kumadio.Core.Common;
using Kumadio.Core.Common.Interfaces.Base;
using Kumadio.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Kumadio.Infrastructure.Repositories.Base
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action)
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

        public async Task<Result<T>> ExecuteInTransactionAsync<T>(Func<Task<Result<T>>> action)
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
