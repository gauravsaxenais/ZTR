namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using ZTR.Framework.DataAccess;
    using EnsureThat;
    using FluentValidation;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class BaseCommandManager<TDbContext, TErrorCode, TEntity> : Manager
        where TDbContext : DbContext
        where TErrorCode : struct, Enum
        where TEntity : class, IEntityWithId
    {
        public BaseCommandManager(TDbContext databaseContext, ILogger<BaseCommandManager<TDbContext, TErrorCode, TEntity>> logger)
            : base(logger)
        {
            EnsureArg.IsNotNull(databaseContext, nameof(databaseContext));

            DatabaseContext = databaseContext;
        }

        protected TDbContext DatabaseContext { get; }

        protected virtual async Task<ManagerResponse<TErrorCode>> DeleteByExpressionAsync<T>(IEnumerable<T> keys, Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                EnsureArg.IsNotNull(predicate, nameof(predicate));

                var entities = await DeleteEntityQueryAsync(predicate).ConfigureAwait(false);

                var errorRecords = await DeleteValidationAsync(keys, entities).ConfigureAwait(false);

                if (errorRecords.Any())
                {
                    return new ManagerResponse<TErrorCode>(errorRecords);
                }
                else
                {
                    DatabaseContext.RemoveRange(entities);

                    await DatabaseContext.SaveChangesAsync().ConfigureAwait(false);

                    return new ManagerResponse<TErrorCode>(entities.Select(x => x.Id));
                }
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, nameof(DeleteByExpressionAsync));
                return new ManagerResponse<TErrorCode>(ex);
            }
        }

        protected virtual async Task<ErrorRecords<TErrorCode>> DeleteValidationAsync<T>(IEnumerable<T> keys, IList<TEntity> entities)
        {
            return await Task.FromResult(new ErrorRecords<TErrorCode>()).ConfigureAwait(false);
        }

        protected virtual async Task<List<TEntity>> DeleteEntityQueryAsync(Expression<Func<TEntity, bool>> predicate)
        {
            EnsureArg.IsNotNull(predicate, nameof(predicate));

            return await DatabaseContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}
