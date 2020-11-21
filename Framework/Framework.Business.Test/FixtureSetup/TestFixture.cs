namespace ZTR.Framework.Business.Test.FixtureSetup
{
    using System.Reflection;
    using ZTR.Framework.Business;
    using ZTR.Framework.Configuration;
    using ZTR.Framework.Test;
    using Microsoft.Extensions.Hosting;

    public class TestFixture : TestFixtureBase<TestFixture>
    {
        protected override IHostBuilder ConfigureHost()
        {
            return new HostBuilder()
                .DefaultAppConfiguration(Assembly.GetAssembly(typeof(ApplicationOptions)))
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAutoMapper(Assembly.GetAssembly(typeof(TestFixture)));
                    services.AddManagers(Assembly.GetAssembly(typeof(TestFixture)));
                    services.AddFakers(Assembly.GetAssembly(typeof(TestFixture)));
                });
        }
    }
}
