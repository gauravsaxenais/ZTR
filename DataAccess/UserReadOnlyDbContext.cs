namespace DataAccess
{
    using Microsoft.EntityFrameworkCore;
    using System.Reflection;
    using ZTR.Framework.DataAccess;

    public partial class UserReadOnlyDbContext : BaseReadOnlyDbContext<UserReadOnlyDbContext>
    {
        public const string DefaultSchemaName = "user";

        public UserReadOnlyDbContext(DbContextOptions<UserReadOnlyDbContext> options)
            : base(options)
        {
        }

        public override string SchemaName
        {
            get { return DefaultSchemaName; }
        }

        public override Assembly[] GetTypeAssemblies()
        {
            return new[] { typeof(UserReadOnlyDbContext).Assembly };
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .ApplySnakeCase();
        }
    }
}
