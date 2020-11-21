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

    public abstract class MasterKeyCodeQueryManager<TDbContext, TEntity, TReadModel> : MasterKeyQueryManager<TDbContext, TEntity, TReadModel>, IMasterKeyCodeQueryManager<TReadModel>
        where TDbContext : DbContext
        where TEntity : class, IEntityWithId, IEntityWithCode, IEntityWithMasterKey
        where TReadModel : class, IModelWithCode, IModelWithMasterKey
    {
        protected MasterKeyCodeQueryManager(TDbContext databaseContext, ILogger<MasterKeyCodeQueryManager<TDbContext, TEntity, TReadModel>> logger, IMapper mapper)
            : base(databaseContext, logger, mapper)
        {
        }

        public virtual async Task<IEnumerable<TReadModel>> GetByCodeAsync(string code, params string[] codes)
        {
            EnsureArg.IsNotNull(code, nameof(code));
            return await GetByCodeAsync(codes.Prepend(code)).ConfigureAwait(false);
        }

        public virtual async Task<IEnumerable<TReadModel>> GetByCodeAsync(IEnumerable<string> codes)
        {
            EnsureArg.IsNotNull(codes, nameof(codes));
            EnsureArgExtensions.HasItems(codes, nameof(codes));
            return await GetByExpressionAsync(x => codes.Contains(x.Code)).ConfigureAwait(false);
        }
    }
}
