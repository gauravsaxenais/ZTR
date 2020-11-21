namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using DataAccess;
    using EnsureThat;
    using Managers.Query;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class QueryManager<TDbContext, TEntity, TReadModel> : BaseQueryManagerExpression<TDbContext, TEntity, TReadModel>, IQueryManager<TReadModel>
        where TDbContext : DbContext
        where TEntity : class, IEntityWithId
        where TReadModel : class
    {
        protected QueryManager(TDbContext databaseContext, ILogger<QueryManager<TDbContext, TEntity, TReadModel>> logger, IMapper mapper)
            : base(databaseContext, logger, mapper)
        {
        }

        public virtual async Task<IEnumerable<TReadModel>> GetByIdAsync(long id, params long[] ids)
        {
            EnsureArg.IsGt(id, 0, nameof(id));
            return await GetByIdAsync(ids.Prepend(id)).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TReadModel>> GetByIdAsync(IEnumerable<long> ids)
        {
            EnsureArg.IsNotNull(ids, nameof(ids));
            EnsureArgExtensions.HasItems(ids, nameof(ids));
            return await GetByExpressionAsync(x => ids.Contains(x.Id)).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TReadModel>> GetAllAsync()
        {
            return await GetByExpressionAsync(x => true).ConfigureAwait(false);
        }

        protected virtual Task QueryAfterMapAsync(IList<TReadModel> models, IList<TEntity> entities)
        {
            return Task.CompletedTask;
        }

        protected virtual async Task<IEnumerable<TReadModel>> GetByExpressionAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var result = await GetByQueryExpressionAsync(predicate).ConfigureAwait(false);
            await QueryAfterMapAsync(result.Models, result.Entities).ConfigureAwait(false);
            return result.Models;
        }
    }
}
