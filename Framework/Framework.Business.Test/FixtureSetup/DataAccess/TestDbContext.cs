namespace ZTR.Framework.Business.Test.FixtureSetup.DataAccess
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using ZTR.Framework.DataAccess;

    public class TestDbContext : BaseDbContext<TestDbContext>
    {
        public const string DefaultSchemaName = "framework_business_test";

        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public TestDbContext()
            : base()
        {
        }

        public override string SchemaName => DefaultSchemaName;

        public override Assembly[] GetTypeAssemblies() => new[] { Assembly.GetAssembly(typeof(TestDbContext)) };

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .ApplySnakeCase();
        }
    }
}
