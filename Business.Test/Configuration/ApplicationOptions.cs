namespace Business.Test.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using ZTR.Framework.Business.File;
    using ZTR.Framework.Configuration;

    public sealed class ApplicationOptions : ConfigurationOptions
    {
        private readonly string _databaseSuffix = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture);
        private string _testConnectionString;
        private string _testReadOnlyConnectionString;

        public string TestConnectionString
        {
            get
            {
                if (_testConnectionString != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, _testConnectionString, _databaseSuffix);
                }

                return null;
            }

            set
            {
                _testConnectionString = value;
            }
        }

        public string TestReadOnlyConnectionString
        {
            get
            {
                if (_testReadOnlyConnectionString != null)
                {
                    return string.Format(CultureInfo.InvariantCulture, _testReadOnlyConnectionString, _databaseSuffix);
                }

                return null;
            }

            set
            {
                _testReadOnlyConnectionString = value;
            }
        }

        public IEnumerable<FtpConfiguration> DigitalAdvertisingFtpConnections { get; set; }

        public IEnumerable<FtpConfiguration> ChatFtpConnections { get; set; }

        public IEnumerable<FtpConfiguration> TrafficFtpConnections { get; set; }

        public IEnumerable<FtpConfiguration> ReputationFtpConnections { get; set; }

        public Guid TenantMasterKey { get; set; }
    }
}
