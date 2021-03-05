namespace Business.GitRepository
{
    using Business.GitRepository.ZTR.M7Config.Business;
    using global::ZTR.M7Config.Business.GitRepository.Interfaces;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AddGitConnections(this IServiceCollection services)
        {
            // Service ZTR.M7Config.Business. for git repository.
            services.AddTransient<IModuleServiceManager, ModuleServiceManager>();
            services.AddTransient<IDeviceServiceManager, DeviceServiceManager>();
            services.AddTransient<IBlockServiceManager, BlockServiceManager>();
            services.AddTransient<IFirmwareVersionServiceManager, FirmwareVersionServiceManager>();
        }
    }
}
