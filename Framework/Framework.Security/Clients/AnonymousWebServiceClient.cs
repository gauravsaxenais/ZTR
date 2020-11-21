namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using ZTR.Framework.Security.Clients;

    public class AnonymousWebServiceClient : WebServiceClientBase, IAnonymousWebServiceClient
    {
        public AnonymousWebServiceClient(IHttpClientFactory httpClientFactory)
            : base(httpClientFactory)
        {
        }

        public async Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, CancellationToken cancellationToken, long? apiTimeoutInMinutes = null)
        {
            return await SendRequestAsync(serviceUri, httpVerb, httpContentType, content, apiTimeoutInMinutes, null, null, cancellationToken).ConfigureAwait(false);
        }
    }
}
