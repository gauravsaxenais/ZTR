namespace ZTR.Framework.Business.Managers.Query
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using AutoMapper;
    using DataAccess;
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class BaseQueryManagerExpression<TDbContext, TEntity, TReadModel> : BaseQueryManager<TDbContext>
        where TDbContext : DbContext
        where TEntity : class, IEntityWithId
        where TReadModel : class
    {
        protected BaseQueryManagerExpression(TDbContext databaseContext, ILogger<BaseQueryManagerExpression<TDbContext, TEntity, TReadModel>> logger, IMapper mapper)
            : base(databaseContext, logger)
        {
            EnsureArg.IsNotNull(mapper, nameof(mapper));
            Mapper = mapper;
        }

        protected IMapper Mapper { get; }

        protected virtual async Task<List<TEntity>> EntityQueryAsync(Expression<Func<TEntity, bool>> predicate)
        {
            EnsureArg.IsNotNull(predicate, nameof(predicate));

            return await DatabaseContext.Set<TEntity>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync()
                .ConfigureAwait(false);
        }

#pragma warning disable SA1009
        protected virtual async Task<(List<TReadModel> Models, List<TEntity> Entities)> GetByQueryExpressionAsync(Expression<Func<TEntity, bool>> predicate)
        {
#pragma warning restore SA1009

            EnsureArg.IsNotNull(predicate, nameof(predicate));
            var results = new List<TReadModel>();
            var entities = await EntityQueryAsync(predicate).ConfigureAwait(false);
            var models = Mapper.Map(entities, results);
            return new ValueTuple<List<TReadModel>, List<TEntity>>(models, entities);
        }
    }
}
