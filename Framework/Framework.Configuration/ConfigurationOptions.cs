namespace ZTR.Framework.Configuration
{
    using System;
    using Builder.Abstraction;

    #pragma warning disable CS1591
    public abstract class ConfigurationOptions : IConfigurationOptions
    {
        private const string Suffix = "Options";

        private string _sectionName;

        public string SectionName
        {
            get => GetSectionName();

            set => _sectionName = value;
        }

        private string GetSectionName()
        {
            if (string.IsNullOrWhiteSpace(_sectionName))
            {
                var className = GetType().Name;

                if (className.EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
                {
                    _sectionName = className.Substring(0, className.Length - Suffix.Length);
                }
            }

            return _sectionName;
        }
    }
    #pragma warning restore CS1591
}
