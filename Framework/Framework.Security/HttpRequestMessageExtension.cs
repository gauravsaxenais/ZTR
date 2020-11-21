namespace ZTR.Framework.Security
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using EnsureThat;
    using Microsoft.Rest;

    public static class HttpRequestMessageExtension
    {
        public static HttpRequestMessage ConfigHttpRequestMessage(this HttpRequestMessage httpRequestMessage, Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content)
        {
            content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(httpContentType);
            httpRequestMessage.Content = content;
            httpRequestMessage.Method = new HttpMethod(httpVerb.ToString());
            httpRequestMessage.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse(httpContentType));
            httpRequestMessage.RequestUri = serviceUri;

            return httpRequestMessage;
        }

        public static async Task<HttpRequestMessage> ConfigClientCredentialsHttpRequestMessageAsync(this HttpRequestMessage httpRequestMessage, ClientCredentialsTokens clientCredentialsTokens, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(clientCredentialsTokens, nameof(clientCredentialsTokens));

            var accessToken = await clientCredentialsTokens.GetAccessTokenAsync().ConfigureAwait(false);
            var credentials = new TokenCredentials(accessToken);

            if (credentials == null)
            {
                throw new InvalidOperationException("ConfigClientCredentialsHttpRequestMessageAsync: ClientCredentials token can not be null.");
            }

            //// This sets the Bearer authentication token
            cancellationToken.ThrowIfCancellationRequested();
            await credentials.ProcessHttpRequestAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);

            return httpRequestMessage;
        }

        public static async Task<HttpRequestMessage> ConfigClientCredentialsHttpRequestMessageAsync(this HttpRequestMessage httpRequestMessage, ClientCredentialsTokens clientCredentialsTokens, IApplicationUser imperonateUser, CancellationToken cancellationToken)
        {
            EnsureArg.IsNotNull(clientCredentialsTokens, nameof(clientCredentialsTokens));

            var accessToken = await clientCredentialsTokens.GetAccessTokenAsync().ConfigureAwait(false);
            var credentials = new TokenCredentials(accessToken);

            if (imperonateUser?.UserId != default)
            {
                httpRequestMessage.Headers.Add(CustomClaimTypes.UserId, imperonateUser.UserId.ToString(CultureInfo.InvariantCulture));
            }

            if (imperonateUser?.CurrentTimeZone != default)
            {
                httpRequestMessage.Headers.Add(CustomClaimTypes.TimeZone, imperonateUser.CurrentTimeZone);
            }

            if (imperonateUser?.FormatIsoCode != default)
            {
                httpRequestMessage.Headers.Add(CustomClaimTypes.FormatIsoCode, imperonateUser.FormatIsoCode);
            }

            if (credentials == null)
            {
                throw new InvalidOperationException("ConfigClientCredentialsHttpRequestMessageAsync: ClientCredentials token can not be null.");
            }

            //// This sets the Bearer authentication token
            cancellationToken.ThrowIfCancellationRequested();
            await credentials.ProcessHttpRequestAsync(httpRequestMessage, cancellationToken).ConfigureAwait(false);

            return httpRequestMessage;
        }
    }
}
