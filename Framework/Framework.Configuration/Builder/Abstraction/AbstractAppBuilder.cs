namespace ZTR.Framework.Configuration.Builder.Abstraction
{
    using System;
    using System.Collections.Generic;
    using Extension;
    using Microsoft.Extensions.Configuration;

    #pragma warning disable CS1591
    public abstract class AbstractAppBuilder
    {
        public virtual void ConfigureApp(string hostingEnvironmentName, IConfigurationBuilder configurationBuilder, IEnumerable<Type> types, List<IConfigurationOptions> configurationOptions, string[] args = null)
        {
            configurationBuilder.AddAppConfiguration(hostingEnvironmentName, args);
            var configurationRoot = configurationBuilder.Build();
            foreach (var optionType in types)
            {
                var instance = (ConfigurationOptions)Activator.CreateInstance(optionType);
                configurationRoot.GetSection(instance.SectionName).Bind(instance);
                configurationOptions.Add(instance);
            }
        }
    }
    #pragma warning restore CS1591
}
