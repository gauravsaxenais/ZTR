namespace ZTR.Framework.Business.Test
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using ZTR.Framework.Configuration;

    public class ApplicationOptions : ConfigurationOptions
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
    }
}
