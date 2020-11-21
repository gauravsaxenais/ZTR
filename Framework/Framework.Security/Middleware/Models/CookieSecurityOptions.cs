namespace ZTR.Framework.Security
{
    using System;
    using System.Collections.Generic;

    public class CookieSecurityOptions
    {
        public string HybridClientId { get; set; }

        public Guid HybridClientSecret { get; set; }

        public string CookieName { get; set; }

        public Uri SignedOutRedirectUri { get; set; }

        public IEnumerable<string> Scopes { get; set; }
    }
}
