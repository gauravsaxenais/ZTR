namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using ZTR.Framework.Security.Clients;
    using EnsureThat;

    public class BearerTokenWebServiceClient : WebServiceClientBase, IBearerTokenWebServiceClient
    {
        public BearerTokenWebServiceClient(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, string bearerToken, CancellationToken cancellationToken, long? apiTimeoutInMinutes = null)
        {
            EnsureArg.IsNotNullOrWhiteSpace(bearerToken, nameof(bearerToken));

            return await SendRequestAsync(serviceUri, httpVerb, httpContentType, content, apiTimeoutInMinutes, null, c => AttachBearerToken(c, bearerToken), cancellationToken).ConfigureAwait(false);
        }

        private static Task AttachBearerToken(HttpClient client, string bearerToken)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(SecurityConstants.Bearer, bearerToken);
            return Task.CompletedTask;
        }
    }
}
