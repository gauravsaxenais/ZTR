namespace ZTR.Framework.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builder.Abstraction;
    using Builder.Extension;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    #pragma warning disable CS1591
    public class HostingBuilder : AbstractAppBuilder, IAppBuilder<IHostBuilder>
    {
        private const string EnvironmentVariablePrefix = "ASPNETCORE_";

        public IHostBuilder ConfigureHostConfiguration(IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureHostConfiguration(configuration =>
            {
                configuration.AddEnvironmentVariables(prefix: EnvironmentVariablePrefix);
            });
        }

        public IHostBuilder ConfigureAppConfiguration(IHostBuilder hostBuilder, IEnumerable<Type> types, List<IConfigurationOptions> options, string[] args)
        {
            return hostBuilder.ConfigureAppConfiguration((webHostBuilderContext, builderConfiguration) =>
            {
                ConfigureApp(webHostBuilderContext.HostingEnvironment.EnvironmentName, builderConfiguration, types, options, args);
            });
        }

        public IHostBuilder ConfigureLogging(IHostBuilder hostBuilder, string loggingSectionName)
        {
            return hostBuilder.ConfigureLogging((webHostBuilderContext, logging) =>
            {
                var configuration = webHostBuilderContext.Configuration.GetSection(loggingSectionName);
                logging.AddLogging(configuration);
            });
        }

        public IHostBuilder ConfigureServices(IHostBuilder hostBuilder, List<IConfigurationOptions> configurationOptions)
        {
            return hostBuilder.ConfigureServices((hostingContext, services) =>
            {
                configurationOptions.ForEach(x => services.AddSingleton(x.GetType(), x));
            });
        }
    }
    #pragma warning restore CS1591
}
