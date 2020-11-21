namespace ZTR.Framework.Security.Authorization
{
    public interface IAccessTokenProvider
    {
        void AddCredentials(string accessToken);
    }
}
