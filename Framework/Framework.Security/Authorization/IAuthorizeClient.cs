namespace ZTR.Framework.Security.Authorization
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IAuthorizeClient : IAccessTokenProvider
    {
        Task<IEnumerable<SecurityRightIdentifierModel>> GetUserRights(string applicationModuleCode);
    }
}
