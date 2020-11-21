namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using EnsureThat;

    public abstract class WebServiceClientBase : IWebServiceClientBase
    {
        public WebServiceClientBase(IHttpClientFactory httpClientFactory)
        {
            EnsureArg.IsNotNull(httpClientFactory, nameof(httpClientFactory));

            HttpClientFactory = httpClientFactory;
        }

        public Func<HttpRequestMessage, Task> RequestHandler { get; set; }

        public Func<HttpClient, Task> ClientHandler { get; set; }

        protected IHttpClientFactory HttpClientFactory { get; }

        protected async Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, long? apiTimeoutInMinutes, Func<HttpRequestMessage, Task> configRequestMessage, Func<HttpClient, Task> configClient, CancellationToken cancellationToken)
        {
            using (var request = await GetRequestMessage(cancellationToken).ConfigureAwait(false))
            {
                request.ConfigHttpRequestMessage(serviceUri, httpVerb, httpContentType, content);

                if (configRequestMessage != null)
                {
                    await configRequestMessage(request).ConfigureAwait(false);
                }

                if (RequestHandler != null)
                {
                    await RequestHandler(request).ConfigureAwait(false);
                }

                using (HttpClient client = HttpClientFactory.CreateClient(serviceUri.AbsoluteUri))
                {
                    client.ConfigHttpClient(apiTimeoutInMinutes, ServiceClientConstants.UserAgentHeaders);

                    if (configClient != null)
                    {
                        await configClient(client).ConfigureAwait(false);
                    }

                    if (ClientHandler != null)
                    {
                        await ClientHandler(client).ConfigureAwait(false);
                    }

                    return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        protected virtual Task<HttpRequestMessage> GetRequestMessage(CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpRequestMessage());
        }
    }
}
