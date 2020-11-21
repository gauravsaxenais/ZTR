namespace ZTR.Business.Test
{
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Moq;
    using System;
    using System.Net;
    using ZTR.Framework.Test;

    public sealed class IntegrationFixture : IntegrationFixtureBaseStandalone<IntegrationFixture>
    {
        protected override IHostBuilder ConfigureHost()
        {
            return new HostBuilder()
        //        .DefaultAppConfiguration(new[] { typeof(ApplicationOptions).Assembly, typeof(SecurityOptions).Assembly, typeof(RedisOptions).Assembly })
        //        .ConfigureServices((hostContext, services) =>
        //        {
        //            var serviceProvider = services.BuildServiceProvider();

        //            services.AddAutoMapper(typeof(MakeMappingProfile).Assembly);
        //            services.AddManagers(typeof(MakeCommandManager).Assembly, ServiceLifetime.Transient);
        //            services.AddFakers(typeof(IntegrationFixture).Assembly);

        //            services.AddTransient<LeadSaleMatchCommandManager>();
        //            services.AddTransient<ModelValidator<LeadSaleMatchCreateModel>>();
        //            services.AddTransient<ModelValidator<LeadSaleMatchUpdateModel>>();

        //            services.AddTransient(
        //             x =>
        //             {
        //                 var client = new Mock<IBlobManager>();
        //                 return client.Object;
        //             });
        //            services.AddScoped<DataCacheQueryManager>();

        //            services.AddTransient(
        //                sp =>
        //                {
        //                    var applicationOptions = sp.GetRequiredService<ApplicationOptions>();
        //                    return applicationOptions.DispositionSyncSettings;
        //                });

        //            services.AddTransient<IHostedService, DatabaseMigrationService<ReportingDataDbContext>>();

        //            services.AddScoped<Mock<IDealerClient>>();

        //            services.AddTransient(
        //                x =>
        //                {
        //                    var client = new Mock<ILeadProcessingClient>();
        //                    return client.Object;
        //                });

        //            services.AddScoped(
        //                x =>
        //                {
        //                    var client = x.GetRequiredService<Mock<IDealerClient>>();
        //                    return client.Object;
        //                });

        //            services.AddTransient(
        //                x =>
        //                {
        //                    var client = new Mock<IBrandClient>();
        //                    return client.Object;
        //                });

        //            services.AddTransient(
        //                x =>
        //                {
        //                    var client = new Mock<IGeographyClient>();
        //                    return client.Object;
        //                });

        //            services.AddTransient(
        //               x =>
        //               {
        //                   var client = new Mock<IClientOrganizationClient>();
        //                   return client.Object;
        //               });

        //            services.AddScoped(
        //               x =>
        //               {
        //                   var client = new Mock<IBlobManager>();
        //                   return client.Object;
        //               });

        //            services.AddTransient<LeadSalesMatchSettings>();

        //            services.AddScoped(
        //               x =>
        //               {
        //                   var client = new Mock<IBlobManager>();
        //                   return client.Object;
        //               });

        //            services.AddPivot(typeof(IPivotTypeResolver).Assembly);

        //            services.AddSingleton<IMvcJsonSerializationSettings, MvcJsonSerializerSettings>();

        //            var redisOptions = serviceProvider.GetRequiredService<RedisOptions>();

        //            services.AddStackExchangeRedisCache(options =>
        //            {
        //                options.InstanceName = redisOptions.InstanceName;
        //                options.ConfigurationOptions = new StackExchange.Redis.ConfigurationOptions();
        //                switch (redisOptions.RetryPolicy)
        //                {
        //                    case RedisReconnectRetryPolicy.Linear:
        //                        options.ConfigurationOptions.ReconnectRetryPolicy = new LinearRetry(redisOptions.ConnectRetryIntervalInMilliseconds);
        //                        break;

        //                    case RedisReconnectRetryPolicy.Exponential:
        //                        options.ConfigurationOptions.ReconnectRetryPolicy = new ExponentialRetry(redisOptions.ConnectRetryIntervalInMilliseconds, redisOptions.ConnectRetryMaxDeltaBackOffIntervalInMilliseconds);
        //                        break;
        //                }

        //                options.ConfigurationOptions.EndPoints.Add(new DnsEndPoint(redisOptions.Host, redisOptions.Port));
        //                options.ConfigurationOptions.Password = redisOptions.Password;
        //                options.ConfigurationOptions.Ssl = true;
        //                options.ConfigurationOptions.AbortOnConnectFail = false;
        //            });
        //            services.AddSingleton<MetricsCacheManager>();
        //            services.AddHttpContextAccessor();
        //            services.AddScoped(sp =>
        //            {
        //                var applicationOption = sp.GetRequiredService<ApplicationOptions>();

        //                var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
        //                var httpContext = httpContextAccessor.HttpContext;
        //                var securityOptions = sp.GetRequiredService<SecurityOptions>();

        //                var userId = httpContext.GetUserId();
        //                var userFormatIsoCode = httpContext.GetUserFormatIsoCode();
        //                var applicationModuleCode = securityOptions.ApplicationModuleCode;
        //                var tenantMasterKey = applicationOption.TenantMasterKey;
        //                var userBearToken = new Lazy<System.Threading.Tasks.Task<string>>(async () =>
        //                {
        //                    var token = await httpContext.GetTokenAsync().ConfigureAwait(false);
        //                    return token;
        //                });
        //                var cacheContext = new MetricsContext(tenantMasterKey, userId, userFormatIsoCode, applicationModuleCode, userBearToken);

        //                return cacheContext;
        //            });
            ;
        }
    }
}
