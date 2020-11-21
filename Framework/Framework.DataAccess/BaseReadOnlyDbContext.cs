namespace ZTR.Framework.DataAccess
{
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using System;

    public abstract class BaseReadOnlyDbContext<T> : BaseDbContext<T>
        where T : DbContext
    {
        private const string readOnlyErrorMessage = "This context is read-only.";

        public BaseReadOnlyDbContext(DbContextOptions<T> options)
            : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        #region SaveChanges overrides - ensures read-only

        public override int SaveChanges()
        {
            throw new InvalidOperationException(readOnlyErrorMessage);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new InvalidOperationException(readOnlyErrorMessage);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException(readOnlyErrorMessage);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException(readOnlyErrorMessage);
        }

        #endregion
    }
}
