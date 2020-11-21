namespace ZTR.Framework.Security
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityServer4.Models;
    using IdentityServer4.Stores;

    public static class SecurityExtensions
    {
        public static async Task<bool> IsPkceClientAsync(this IClientStore store, string clientId)
        {
            if (!string.IsNullOrWhiteSpace(clientId))
            {
                var client = await store.FindEnabledClientByIdAsync(clientId).ConfigureAwait(false);
                return client?.RequirePkce == true;
            }

            return false;
        }

        public static bool IsRecoveringPassword(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.HasClaim(p => p.Type == CustomClaimTypes.IsPasswordRecovery);
        }

        public static string GuidToString(this Guid masterKey)
        {
            return masterKey.ToString().ToUpperInvariant();
        }

        public static string GetSecretHash(Guid secret)
        {
            return GuidToString(secret).Sha256();
        }
    }
}
