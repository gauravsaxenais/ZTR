namespace ZTR.Framework.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builder.Abstraction;
    using Builder.Extension;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;

    #pragma warning disable CS1591
    public class WebHostingBuilder : AbstractAppBuilder, IAppBuilder<IWebHostBuilder>
    {
        public IWebHostBuilder ConfigureHostConfiguration(IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder;
        }

        public IWebHostBuilder ConfigureAppConfiguration(IWebHostBuilder webHostBuilder, IEnumerable<Type> types, List<IConfigurationOptions> configurationOptions, string[] args)
        {
            return webHostBuilder.ConfigureAppConfiguration((webHostBuilderContext, builderConfiguration) =>
            {
                ConfigureApp(webHostBuilderContext.HostingEnvironment.EnvironmentName, builderConfiguration, types, configurationOptions, args);
            });
        }
        public IWebHostBuilder ConfigureLogging(IWebHostBuilder webHostBuilder, string loggingSectionName = "Logging")
        {
            return webHostBuilder.ConfigureServices((webHostBuilderContext, collection) =>
            {
                var configuration = webHostBuilderContext.Configuration.GetSection(loggingSectionName);

                collection.AddLogging(builder =>
                {
                    builder.AddLogging(configuration);
                });
            });
        }

        public IWebHostBuilder ConfigureServices(IWebHostBuilder webHostBuilder, List<IConfigurationOptions> configurationOptions)
        {
            return webHostBuilder.ConfigureServices((hostingContext, services) =>
            {
                configurationOptions.ForEach(x => services.AddSingleton(x.GetType(), x));
            });
        }
    }
    #pragma warning restore CS1591
}
