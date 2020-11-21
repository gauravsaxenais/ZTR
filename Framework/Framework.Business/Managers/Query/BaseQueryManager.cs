namespace ZTR.Framework.Business
{
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    public abstract class BaseQueryManager<TDbContext> : Manager
        where TDbContext : DbContext
    {
        protected BaseQueryManager(TDbContext databaseContext, ILogger<BaseQueryManager<TDbContext>> logger)
            : base(logger)
        {
            EnsureArg.IsNotNull(databaseContext, nameof(databaseContext));

            DatabaseContext = databaseContext;
        }

        protected TDbContext DatabaseContext { get; private set; }
    }
}
