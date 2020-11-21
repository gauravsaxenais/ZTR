namespace ZTR.Framework.DataAccess
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using ZTR.Framework.DataAccess.Entities.Audit;

    public abstract class BaseDbContext<T> : DbContext
        where T : DbContext
    {
        public BaseDbContext(DbContextOptions<T> options)
            : base(options)
        {
        }

        protected BaseDbContext(DbContextOptions options)
            : base(options)
        {
        }

        protected BaseDbContext()
            : base()
        {
        }

        public abstract string SchemaName { get; }

        public abstract Assembly[] GetTypeAssemblies();

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetAuditableProperties();

            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetAuditableProperties();

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            SetAuditableProperties();

            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetAuditableProperties();

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        private void SetAuditableProperties()
        {
            // get entries that are being Added or Updated

            var modifiedEntries = ChangeTracker.Entries()
                .Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entityEntry in modifiedEntries)
            {
                var now = DateTimeOffset.UtcNow;
                // If state is Added, set DateCreated and CreatedBy properties
                if (entityEntry.State == EntityState.Added)
                {
                    ((AuditableEntity)entityEntry.Entity).DateCreated = now;
                }

                // In any case we always want to set the properties
                // ModifiedAt and ModifiedBy
                ((AuditableEntity)entityEntry.Entity).DateModified = now;
            }
        }
    }
}
