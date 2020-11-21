namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Authorization;

    public interface IInternalWebServiceClient : IClientCredentialsProvider, IAccessTokenProvider
    {
        string BaseUrl { get; set; }

        Task<HttpResponseMessage> SendRequestAsync(Uri serviceUri, HttpVerb httpVerb, string httpContentType, StringContent content, CancellationToken cancellationToken, bool isInternalUrl = false, long? apiTimeoutInMinutes = null);
    }
}
