namespace ZTR.Framework.Security
{
    using System;

    public interface IClientCredentialsProvider
    {
        long UserId { get; set; }

        string LanguageIsoCode { get; set; }

        string FormatIsoCode { get; set; }

        string TimeZoneCode { get; set; }

        void AddCredentials(ClientCredentialsTokens clientCredentialsTokens);
    }
}
