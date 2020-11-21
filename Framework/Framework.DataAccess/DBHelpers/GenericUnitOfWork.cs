namespace ZTR.Framework.DataAccess.DBHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Generic class for db queries.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    public class GenericUnitOfWork<TContext> : IDisposable where TContext : DbContext
    {
        /// <summary>
        /// Generic unit of work contains a dictionary of repositories.
        /// </summary>
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        /// <summary>
        /// A constructor with a generic dbcontext. This avoids coupling with a particular dbcontext.
        /// </summary>
        /// <param name="context"></param>
        public GenericUnitOfWork(TContext context)
        {
            Context = context;
        }
        
        /// <summary>
        /// context property
        /// </summary>
        public TContext Context { get; } = null;
        
        /// <summary>
        /// Create context connection of particular table in entity.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns>Return context object.</returns>
        public GenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories.Keys.Contains(typeof(TEntity)))
            {
                return _repositories[typeof(TEntity)] as GenericRepository<TEntity>;
            }

            GenericRepository<TEntity> repo = new GenericRepository<TEntity>(Context);
            _repositories.Add(typeof(TEntity), repo);
            return repo;
        }
        /// <summary>
        /// Save changed data in DB.
        /// </summary>
        /// <returns>Return number of record saved.</returns>
        public int SaveChanges()
        {
            return Context.SaveChanges();
        }
        /// <summary>
        /// Save changed data in DB asyncronously.
        /// </summary>
        /// <returns>Return number of record saved.</returns>
        public Task<int> SaveChangesAsync()
        {
            return Context.SaveChangesAsync();
        }
        /// <summary>
        /// Disposed bool variable initialise.
        /// </summary>
        private bool disposed = false;
        /// <summary>
        /// Method to dispose DB connection.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            disposed = true;
        }
        /// <summary>
        /// Method for garbage collection of unused objects.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

