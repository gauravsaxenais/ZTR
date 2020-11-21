namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class InternalWebServiceClient : ServiceClientBase<InternalWebServiceClient>, IInternalWebServiceClient
    {
        private const string UserAgentHeaders = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) (compatible; MSIE 6.0; Windows NT 5.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private readonly IHttpClientFactory _httpClientFactory;

        public InternalWebServiceClient(string baseUrl, IHttpClientFactory httpClientFactory)
        {
            BaseUrl = baseUrl;
            _httpClientFactory = httpClientFactory;
        }

        public string BaseUrl { get; set; }

        public async Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, CancellationToken cancellationToken, bool isInternalUrl = false, long? apiTimeoutInMinutes = null)
        {
            using (var request = await GetRequestMessage(isInternalUrl, cancellationToken).ConfigureAwait(false))
            {
                content.Headers.ContentType = System.Net.Http.Headers.MediaTypeHeaderValue.Parse(httpContentType);
                request.Content = content;
                request.Method = new HttpMethod(httpVerb.ToString());
                request.Headers.Accept.Add(System.Net.Http.Headers.MediaTypeWithQualityHeaderValue.Parse(httpContentType));
                request.RequestUri = serviceUri;
                using (HttpClient client = _httpClientFactory.CreateClient(serviceUri.AbsoluteUri))
                {
                    client.Timeout = apiTimeoutInMinutes.HasValue ? TimeSpan.FromMinutes(apiTimeoutInMinutes.Value) : client.Timeout;
                    client.DefaultRequestHeaders.Add("User-Agent", UserAgentHeaders);
                    return await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task<HttpRequestMessage> GetRequestMessage(bool isInternalUrl, CancellationToken cancellationToken)
        {
            if (!isInternalUrl)
            {
                return new HttpRequestMessage();
            }

            return await CreateHttpRequestMessageAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
