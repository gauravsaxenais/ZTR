namespace Service.Configuration
{
    using System;
    using ZTR.Framework.Configuration;

    public sealed class ApplicationOptions : ConfigurationOptions
    {
        public string ConnectionString { get; set; }

        public string ReadOnlyConnectionString { get; set; }

        public Uri ServiceBaseUri { get; set; }
    }
}
