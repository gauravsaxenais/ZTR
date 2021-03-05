namespace Service
{
    using Business.GitRepository;
    using Business.GitRepository.ZTR.M7Config.Business;
    using EnsureThat;
    using Microsoft.Extensions.DependencyInjection;
    using ZTR.Framework.Service;
    using ZTR.M7Config.Business.GitRepository.Interfaces;
    using ZTR.M7Config.Business.Parsers;
    using ZTR.M7Config.Business.RequestHandlers.Interfaces;
    using ZTR.M7Config.Business.RequestHandlers.Managers;

    /// <summary>
    /// Services Collections Extensions.
    /// Inject services in the application here.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>Adds the services.</summary>
        /// <param name="services">The services.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            EnsureArg.IsNotNull(services, nameof(services));

            services.AddScoped<IGitRepositoryManager, GitRepositoryManager>();
            services.AddScoped<IDeviceTypeManager, DeviceTypeManager>();
            services.AddScoped<IFirmwareVersionManager, FirmwareVersionManager>();
            services.AddScoped<IDefaultValueManager, DefaultValueManager>();
            services.AddScoped<IBlockManager, BlockManager>();
            services.AddScoped<IConfigManager, ConfigManager>();
            services.AddScoped<IConfigCreateFromManager, ConfigCreateFromManager>();
            services.AddScoped<ICompatibleFirmwareVersionManager, CompatibleFirmwareVersionManager>();

            services.AddCors(o => o.AddPolicy(ApiConstants.ApiAllowAllOriginsPolicy, builder =>
            {
                builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
            }));

            services.AddConverters();
            services.AddGitConnections();

            return services;
        }
    }
}
