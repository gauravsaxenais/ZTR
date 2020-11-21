namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using EnsureThat;

    /// <summary>
    /// Used to issue Client Credential Service calls between different domains. For example: calling ReportingData end points from Mitsu
    /// </summary>
    /// <example><![CDATA[
    ///     .ConfigureServices((hostContext, services) =>
    ///            {
    ///                 //AddClientCredentialsTokenAuthentication() is required here
    ///                 services.AddClientCredentialsTokenAuthentication();
    ///                 services.AddTransient<IClientCredentialServiceClient, ClientCredentialServiceClient>();
    ///             }
    /// ]]>
    /// </example>
    public class ClientCredentialServiceClient : WebServiceClientBase, IClientCredentialServiceClient
    {
        private readonly ClientCredentialsTokens _clientCredentialsTokens;

        public ClientCredentialServiceClient(IHttpClientFactory httpClientFactory, ClientCredentialsTokens clientCredentialsTokens)
            : base(httpClientFactory)
        {
            EnsureArg.IsNotNull(clientCredentialsTokens, nameof(clientCredentialsTokens));

            _clientCredentialsTokens = clientCredentialsTokens;
        }

        public async Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, CancellationToken cancellationToken, long? apiTimeoutInMinutes = null)
        {
            return await SendRequestAsync(serviceUri, httpVerb, httpContentType, content, apiTimeoutInMinutes, async m => await AttachClientCredentialsTokenAsync(m, cancellationToken).ConfigureAwait(false), null, cancellationToken).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, IApplicationUser impersonateUser, CancellationToken cancellationToken, long? apiTimeoutInMinutes = null)
        {
            return await SendRequestAsync(serviceUri, httpVerb, httpContentType, content, apiTimeoutInMinutes, async m => await AttachClientCredentialsTokenAsync(m, impersonateUser, cancellationToken).ConfigureAwait(false), null, cancellationToken).ConfigureAwait(false);
        }

        private async Task AttachClientCredentialsTokenAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
        {
            await httpRequestMessage.ConfigClientCredentialsHttpRequestMessageAsync(_clientCredentialsTokens, cancellationToken).ConfigureAwait(false);
        }

        private async Task AttachClientCredentialsTokenAsync(HttpRequestMessage httpRequestMessage, IApplicationUser impersonateUser, CancellationToken cancellationToken)
        {
            await httpRequestMessage.ConfigClientCredentialsHttpRequestMessageAsync(_clientCredentialsTokens, impersonateUser, cancellationToken).ConfigureAwait(false);
        }
    }
}
