namespace DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;
    using ZTR.Framework.DataAccess;

    public partial class UserDbContext : BaseDbContext<UserDbContext>
    {
        public const string DefaultSchemaName = "user";

        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        {
        }

        public override string SchemaName
        {
            get { return DefaultSchemaName; }
        }

        public override Assembly[] GetTypeAssemblies()
        {
            return new[] { typeof(UserDbContext).Assembly };
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .ApplySnakeCase();
        }
    }
}
