namespace ZTR.Framework.Security
{
    using System;
    using System.Collections.Generic;

    public class ClientCredentialsSecurityOptions
    {
        public string CredentialsClientId { get; set; }

        public Guid CredentialsClientSecret { get; set; }

        public IEnumerable<string> Scopes { get; set; }
    }
}
