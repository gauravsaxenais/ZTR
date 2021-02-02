namespace Business.GitRepository
{
    using Business.Common.Configuration;
    using Business.GitRepository.Interfaces;
    using Business.GitRepository.Managers;
    using Microsoft.Extensions.DependencyInjection;
    using ZTR.Framework.Configuration;

    public static class ServiceCollectionExtensions
    {
        public static void AddGitConnections(this IServiceCollection services)
        {
            // Service managers for git repository.
            services.AddScoped<IModuleServiceManager, ModuleServiceManager>();
            services.AddScoped<IDeviceServiceManager, DeviceServiceManager>();
            services.AddScoped<IBlockServiceManager, BlockServiceManager>();
            services.AddScoped<IFirmwareVersionServiceManager, FirmwareVersionServiceManager>();
        }
    }
}
