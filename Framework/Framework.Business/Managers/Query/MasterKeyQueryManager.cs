namespace ZTR.Framework.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using DataAccess;
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class MasterKeyQueryManager<TDbContext, TEntity, TReadModel> : QueryManager<TDbContext, TEntity, TReadModel>, IMasterKeyQueryManager<TReadModel>
        where TDbContext : DbContext
        where TEntity : class, IEntityWithId, IEntityWithMasterKey
        where TReadModel : class, IModelWithMasterKey
    {
        protected MasterKeyQueryManager(TDbContext databaseContext, ILogger<MasterKeyQueryManager<TDbContext, TEntity, TReadModel>> logger, IMapper mapper)
            : base(databaseContext, logger, mapper)
        {
        }

        public virtual async Task<IEnumerable<TReadModel>> GetByMasterKeyAsync(Guid masterKey, params Guid[] masterKeys)
        {
            EnsureArg.IsNotDefault(masterKey, nameof(masterKey));
            return await GetByMasterKeyAsync(masterKeys.Prepend(masterKey)).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TReadModel>> GetByMasterKeyAsync(IEnumerable<Guid> masterKeys)
        {
            EnsureArg.IsNotNull(masterKeys, nameof(masterKeys));
            EnsureArgExtensions.HasItems(masterKeys, nameof(masterKeys));
            return await GetByExpressionAsync(x => masterKeys.Contains(x.MasterKey)).ConfigureAwait(false);
        }
    }
}
