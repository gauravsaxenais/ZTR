namespace Service
{
    using DataAccess;
    using EnsureThat;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Converters;
    using Service.Configuration;
    using ZTR.Framework.Business.Models;
    using ZTR.Framework.Configuration;
    using ZTR.Framework.Security;
    using ZTR.Framework.Service;

    public class Startup
    {
        private static ILogger logger;

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
#pragma warning disable CA1822 // Mark members as static
        public void ConfigureServices(IServiceCollection services)
#pragma warning restore CA1822 // Mark members as static
        {
            services.AddControllers();

            services.AddAllowAllOriginsCorsPolicy();

            services.AddMvc()
                .AddNewtonsoftJson(options => options.SerializerSettings.Converters.Add(new StringEnumConverter()))
                .AddXmlSerializerFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            var swaggerAssemblies = new[]
            {
                typeof(Program).Assembly,
                typeof(Model).Assembly,
            };

            services.AddSwaggerWithComments(ApiConstants.ApiName, ApiConstants.ApiVersion, ApiConstants.ApiDescription, swaggerAssemblies);

            services.AddDbContext<UserDbContext>(
                (serviceProvider, options) =>
                {
                    var applicationOptions = serviceProvider.GetRequiredService<ApplicationOptions>();
                    if (ApplicationConfiguration.IsDevelopment)
                    {
                        options.EnableDetailedErrors();
                        options.EnableSensitiveDataLogging();
                    }

                    options.UseNpgsql(applicationOptions.ConnectionString);
                }, ServiceLifetime.Scoped);

            services.AddDbContext<UserReadOnlyDbContext>(
            (serviceProvider, options) =>
            {
                var applicationOptions = serviceProvider.GetRequiredService<ApplicationOptions>();
                if (ApplicationConfiguration.IsDevelopment)
                {
                    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }

                options.UseNpgsql(applicationOptions.ReadOnlyConnectionString);
            }, ServiceLifetime.Scoped);

            // we add our custom services here.
            services.AddServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app)
#pragma warning restore CA1822 // Mark members as static
        {
            EnsureArg.IsNotNull(app);
            if (ApplicationConfiguration.IsDevelopment)
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
                app.UseMiddleware<ForceHttpsMiddleware>();
                app.UseHttpsRedirection();
            }

            // Use routing first, then Cors second.
            app.UseRouting();
            var serviceProviderBuilt = app.ApplicationServices;

            logger = serviceProviderBuilt.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(Program));

            // Use Exception middleware.
            app.AddProblemDetailsSupport();

            // Cors needs to be before MVC and Swagger. Otherwise typescript clients throw cors related exceptions.
            logger.LogWarning($"AllowAllOrigins true, so all origins are allowed");
            logger.LogWarning("Caution: Use this setting in DEVELOPMENT only. In production, grant access to specific origins (websites) that you control and trust to access the API.");

            var securityOptions = app.ApplicationServices.GetRequiredService<SecurityOptions>();

            // Cors needs to be before MVC and Swagger. Otherwise typescript clients throw cors related exceptions.
            if (securityOptions.AllowAllOrigins)
            {
                app.UseCors(ApiConstants.ApiAllowAllOriginsPolicy);
            }

            app.UseSwagger(new[]
            {
                new SwaggerConfigurationModel(ApiConstants.ApiVersion, ApiConstants.ApiName, true),
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
