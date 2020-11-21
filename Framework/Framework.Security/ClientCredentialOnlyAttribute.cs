namespace ZTR.Framework.Security
{
    using Microsoft.AspNetCore.Mvc;

    public class ClientCredentialOnlyAttribute : TypeFilterAttribute
    {
        public ClientCredentialOnlyAttribute()
            : base(typeof(ClientCredentialOnlyFilter))
        {
        }
    }
}
