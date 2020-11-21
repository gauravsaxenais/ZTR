namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;

    public static class HttpClientExtension
    {
        public static HttpClient ConfigHttpClient(this HttpClient client, long? apiTimeoutInMinutes, string userAgentHeaders)
        {
            client.Timeout = apiTimeoutInMinutes.HasValue ? TimeSpan.FromMinutes(apiTimeoutInMinutes.Value) : client.Timeout;
            client.DefaultRequestHeaders.Add("User-Agent", userAgentHeaders);

            return client;
        }

        public static HttpClient AddBearerToken(this HttpClient client, string bearerToken)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SecurityConstants.Bearer, bearerToken);
            return client;
        }
    }
}
