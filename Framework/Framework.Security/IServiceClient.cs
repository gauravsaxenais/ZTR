namespace ZTR.Framework.Security
{
    using ZTR.Framework.Security.Authorization;

    public interface IServiceClient : IClientCredentialsProvider, IAccessTokenProvider
    {
        string BaseUrl { get; set; }
    }
}
