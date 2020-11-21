namespace ZTR.Framework.Security.Clients
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAnonymousWebServiceClient : IWebServiceClientBase
    {
        Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, CancellationToken cancellationToken, long? apiTimeoutInMinutes = null);
    }
}
