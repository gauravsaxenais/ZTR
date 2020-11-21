namespace ZTR.Framework.DataAccess
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using EnsureThat;
    using Microsoft.EntityFrameworkCore;

    public static class ModelBuilderExtensions
    {
        public static ModelBuilder ConfigureTypes(this ModelBuilder modelBuilder, IEnumerable<Assembly> assemblies)
        {
            EnsureArg.IsNotNull(modelBuilder, nameof(modelBuilder));
            EnsureArg.IsNotNull(assemblies, nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                modelBuilder.ApplyConfigurationsFromAssembly(assembly);
            }

            return modelBuilder;
        }

        public static ModelBuilder ConfigureTypes(this ModelBuilder modelBuilder, Assembly assembly, params Assembly[] assemblies)
        {
            EnsureArg.IsNotNull(modelBuilder, nameof(modelBuilder));
            EnsureArg.IsNotNull(assembly, nameof(assembly));

            return ConfigureTypes(modelBuilder, assemblies.Prepend(assembly));
        }

        public static ModelBuilder ApplySnakeCase(this ModelBuilder modelBuilder)
        {
            EnsureArg.IsNotNull(modelBuilder, nameof(modelBuilder));

            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                entity.SetTableName(entity.GetTableName().ToSnakeCase());

                foreach (var property in entity.GetProperties())
                {
                    property.SetColumnName(property.Name.ToSnakeCase());
                }

                foreach (var key in entity.GetKeys())
                {
                    key.SetName(key.GetName().ToSnakeCase());
                }

                foreach (var key in entity.GetForeignKeys())
                {
                    key.PrincipalKey.SetName(key.PrincipalKey.GetName().ToSnakeCase());
                }

                foreach (var index in entity.GetIndexes())
                {
                    index.SetName(index.GetName().ToSnakeCase());
                }
            }

            return modelBuilder;
        }
    }
}
