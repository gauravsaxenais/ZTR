namespace ZTR.Framework.Security
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using ZTR.Framework.Security.Authorization;
    using EnsureThat;
    using Microsoft.Rest;

    // todo: look into disposing it
    public abstract class ServiceClientBase<T> : ServiceClient<T>, IClientCredentialsProvider, IAccessTokenProvider
        where T : ServiceClientBase<T>
    {
        private ClientCredentialsTokens _clientCredentialsTokens = null;
        private string _serviceClientAccessToken = null;

        public long UserId { get; set; }

        public string LanguageIsoCode { get; set; }

        public string FormatIsoCode { get; set; }

        public string TimeZoneCode { get; set; }

        public void AddCredentials(ClientCredentialsTokens clientCredentialsTokens)
        {
            EnsureArg.IsNotNull(clientCredentialsTokens, nameof(clientCredentialsTokens));

            if (_serviceClientAccessToken != null)
            {
                throw new InvalidOperationException("Add credentials can only be called once.");
            }

            _clientCredentialsTokens = clientCredentialsTokens;
        }

        public void AddCredentials(string accessToken)
        {
            EnsureArg.IsNotNull(accessToken, nameof(accessToken));

            if (_clientCredentialsTokens != null)
            {
                throw new InvalidOperationException("Add credentials can only be called once.");
            }

            _serviceClientAccessToken = accessToken;
        }

        // Called by implementing swagger client classes
        // https://stackoverflow.com/questions/40025744/how-to-invoke-a-nswag-client-method-that-needs-bearer-token-on-request-header
        protected async Task<HttpRequestMessage> CreateHttpRequestMessageAsync(CancellationToken cancellationToken)
        {
            ServiceClientCredentials credentials = null;
            if (_clientCredentialsTokens != null)
            {
                var accessToken = await _clientCredentialsTokens.GetAccessTokenAsync().ConfigureAwait(false);
                credentials = new TokenCredentials(accessToken);
            }
            else if (_serviceClientAccessToken != null)
            {
                credentials = new TokenCredentials(_serviceClientAccessToken);
            }

            if (credentials != null)
            {
                credentials.InitializeServiceClient(this);
            }

            var httpRequest = new HttpRequestMessage();

            if (UserId != default)
            {
                httpRequest.Headers.Add(CustomClaimTypes.UserId, UserId.ToString(CultureInfo.InvariantCulture));
            }

            if (LanguageIsoCode != default)
            {
                httpRequest.Headers.Add(CustomClaimTypes.LanguageIsoCode, LanguageIsoCode.ToString(CultureInfo.InvariantCulture));
            }

            if (FormatIsoCode != default)
            {
                httpRequest.Headers.Add(CustomClaimTypes.FormatIsoCode, FormatIsoCode.ToString(CultureInfo.InvariantCulture));
            }

            if (TimeZoneCode != default)
            {
                httpRequest.Headers.Add(CustomClaimTypes.TimeZone, TimeZoneCode.ToString(CultureInfo.InvariantCulture));
            }

            // Set Credentials
            if (credentials != null)
            {
                //// This sets the Bearer authentication token
                cancellationToken.ThrowIfCancellationRequested();
                await credentials.ProcessHttpRequestAsync(httpRequest, cancellationToken).ConfigureAwait(false);
            }

            return httpRequest;
        }
    }
}
