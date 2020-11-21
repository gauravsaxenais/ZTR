namespace ZTR.Framework.Configuration.Builder.Abstraction
{
    using System;
    using System.Collections.Generic;

    #pragma warning disable CS1591
    public interface IAppBuilder<TBuilder>
    {
        TBuilder ConfigureHostConfiguration(TBuilder builder);

        TBuilder ConfigureAppConfiguration(TBuilder builder, IEnumerable<Type> types, List<IConfigurationOptions> configurationOptions, string[] args);

        TBuilder ConfigureLogging(TBuilder builder, string loggingSectionName);

        TBuilder ConfigureServices(TBuilder builder, List<IConfigurationOptions> configurationOptions);
    }
    #pragma warning restore CS1591
}
