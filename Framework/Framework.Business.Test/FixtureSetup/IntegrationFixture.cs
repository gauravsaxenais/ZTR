namespace ZTR.Framework.Business.Test.FixtureSetup
{
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using ZTR.Framework.Business;
    using ZTR.Framework.Business.Test.FixtureSetup.DataAccess;
    using ZTR.Framework.Configuration;
    using ZTR.Framework.Test;

    public class IntegrationFixture : IntegrationFixtureBase<IntegrationFixture, TestDbContext>
    {
        protected override IHostBuilder ConfigureHost()
        {
            return new HostBuilder()
                .DefaultAppConfiguration(Assembly.GetAssembly(typeof(ApplicationOptions)))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddDbContext<TestDbContext>(
                        (serviceProvider, options) =>
                        {
                            var applicationOptions = serviceProvider.GetRequiredService<ApplicationOptions>();
                        }, ServiceLifetime.Transient);
                    services.AddAutoMapper(Assembly.GetAssembly(typeof(IntegrationFixture)));
                    services.AddManagers(Assembly.GetAssembly(typeof(IntegrationFixture)), ServiceLifetime.Transient);
                    services.AddFakers(Assembly.GetAssembly(typeof(IntegrationFixture)));
                });
        }
    }
}
