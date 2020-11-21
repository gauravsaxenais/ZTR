namespace ZTR.Framework.Test
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public abstract class IntegrationFixtureBase<TFixture, TDbContext> : IDisposable, IIntegrationFixture<TFixture>
        where TFixture : class
        where TDbContext : DbContext
    {
        private TDbContext _databaseContext;
        private IHost _host;
        private ILogger _logger;

        protected IntegrationFixtureBase()
        {
            Initialize();
        }

        ~IntegrationFixtureBase()
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
                _databaseContext?.Database.EnsureDeleted();
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
            _databaseContext = ServiceProvider.GetRequiredService<TDbContext>();
            _logger = ServiceProvider.GetRequiredService<ILogger<IntegrationFixtureBase<TFixture, TDbContext>>>();

            var mapperConfiguration = ServiceProvider.GetRequiredService<AutoMapper.IConfigurationProvider>();
            mapperConfiguration.AssertConfigurationIsValid();

            // run host
            _host.Start();
            _host.StopAsync();
        }
    }
}
