namespace ZTR.Framework.Test
{
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public abstract class IntegrationFixtureBaseStandalone<TFixture> : IDisposable, IIntegrationFixture<TFixture>
        where TFixture : class
    {
        private IHost _host;
        private ILogger _logger;

        protected IntegrationFixtureBaseStandalone()
        {
            Initialize();
        }

        ~IntegrationFixtureBaseStandalone()
        {
            Dispose(false);
        }

        public IServiceProvider ServiceProvider { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract IHostBuilder ConfigureHost();

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _host?.Dispose();
            }
        }

        // todo: Combine the startup from the two fixtures to avoid duplication
        private void Initialize()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            var builder = ConfigureHost();

            _host = builder.Build();

            ServiceProvider = _host.Services;
            _logger = ServiceProvider.GetRequiredService<ILogger<IntegrationFixtureBaseStandalone<TFixture>>>();

            var mapperConfiguration = ServiceProvider.GetRequiredService<AutoMapper.IConfigurationProvider>();
            mapperConfiguration.AssertConfigurationIsValid();

            // run host
            _host.Start();
            _host.StopAsync();
        }
    }
}
