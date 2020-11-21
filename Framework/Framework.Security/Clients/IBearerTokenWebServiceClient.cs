namespace ZTR.Framework.Security.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBearerTokenWebServiceClient : IWebServiceClientBase
    {
        Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, string bearerToken, CancellationToken cancellationToken, long? apiTimeoutInMinutes = null);
    }
}
