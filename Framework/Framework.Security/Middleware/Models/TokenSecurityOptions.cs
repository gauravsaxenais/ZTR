namespace ZTR.Framework.Security
{
    using System;
    using System.Collections.Generic;

    public class TokenSecurityOptions
    {
        public string ApiCode { get; set; }

        public Guid ApiSecret { get; set; }

        public string SwaggerClientId { get; set; }

        public Guid SwaggerClientSecret { get; set; }

        public IEnumerable<string> Scopes { get; set; }
    }
}
