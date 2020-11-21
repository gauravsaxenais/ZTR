namespace ZTR.Framework.Configuration
{
    using System;
    using System.Collections.Generic;
    using Builder.Abstraction;

    #pragma warning disable CS1591
    public static class BuilderCreator
    {
        public static TBuilder Construct<TBuilder>(IAppBuilder<TBuilder> appBuilder, TBuilder builder, IEnumerable<Type> optionTypes, List<IConfigurationOptions> options, string[] args = null, string loggingSectionName = "Logging")
        {
            builder = appBuilder.ConfigureHostConfiguration(builder);
            builder = appBuilder.ConfigureAppConfiguration(builder, optionTypes, options, args);
            builder = appBuilder.ConfigureLogging(builder, loggingSectionName);
            builder = appBuilder.ConfigureServices(builder, options);
            return builder;
        }
    }
    #pragma warning restore CS1591
}
