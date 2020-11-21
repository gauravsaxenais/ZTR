namespace ZTR.Framework.Security
{
    using IdentityModel;
    using IdentityServer4;
    using IdentityServer4.AccessTokenValidation;

    public static class SecurityConstants
    {
        /// <summary>
        /// This setting is needed for consuming app to logout during Hybrid flow.
        /// Microsoft.AspNetCore.Mvc SignOut(CookieAuthenticationScheme, Oidc)
        /// </summary>
        public const string CookieAuthenticationScheme = "Cookies";

        /// <summary>
        /// The oidc
        /// </summary>
        public const string Oidc = IdentityServerConstants.ProtocolTypes.OpenIdConnect;

        /// <summary>
        /// The authentication scheme
        /// </summary>
        public const string Bearer = OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer;

        /// <summary>
        /// The authentication scheme
        /// </summary>
        public const string AuthenticationScheme = IdentityServerAuthenticationDefaults.AuthenticationScheme;
    }
}
