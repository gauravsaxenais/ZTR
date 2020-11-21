namespace ZTR.Framework.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityModel.AspNetCore.OAuth2Introspection;
    using IdentityServer4.Extensions;
    using IdentityServer4.Models;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Primitives;
    using Newtonsoft.Json;

    public static class ClaimsPrincipalExtensions
    {
        public static string GetPreferredUserName(this ControllerBase controllerBase)
        {
            return controllerBase.User.GetPreferredUserName();
        }

        public static string GetPreferredUserName(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(JwtClaimTypes.PreferredUserName)?.Value;
        }

        public static string GetLoginProviderCode(this ControllerBase controllerBase)
        {
            return controllerBase.User.GetLoginProviderCode();
        }

        public static string GetLoginProviderCode(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(CustomClaimTypes.LoginProviderCode)?.Value;
        }

        public static string GetLoginProviderTypeCode(this ControllerBase controllerBase)
        {
            return controllerBase.User.GetLoginProviderTypeCode();
        }

        public static string GetLoginProviderTypeCode(this ClaimsPrincipal claimsPrincipal)
        {
            return claimsPrincipal.FindFirst(CustomClaimTypes.LoginProviderTypeCode)?.Value;
        }

        public static long GetUserId(this ControllerBase controllerBase)
        {
            return controllerBase.HttpContext.GetUserId();
        }

        public static long GetUserId(this HttpContext httpContext)
        {
            var subjectId = string.Empty;

            var isClientCredentials = httpContext.User.IsClientCredentialsFlow();
            if (isClientCredentials)
            {
                var parseUser = httpContext.Request.Headers.TryGetValue(CustomClaimTypes.UserId, out StringValues user);
                if (parseUser)
                {
                    subjectId = user.First();
                }
            }
            else
            {
                subjectId = httpContext.User.GetSubjectId();
            }

            var parsedState = long.TryParse(subjectId, out var userId);
            if (!parsedState)
            {
                throw new InvalidOperationException($"Invalid SubjectId");
            }

            return userId;
        }

        public static long GetCompanyId(this ControllerBase controllerBase)
        {
            return controllerBase.User.GetCompanyId();
        }

        public static long GetCompanyId(this ClaimsPrincipal claimsPrincipal)
        {
            var claimValue = claimsPrincipal.FindFirst(CustomClaimTypes.CompanyId)?.Value;

            var parsedState = long.TryParse(claimValue, out var id);

            if (!parsedState)
            {
                throw new InvalidOperationException($"Invalid Company Id");
            }

            return id;
        }

        public static Guid GetCompanyMasterKey(this ControllerBase controllerBase)
        {
            return controllerBase.User.GetCompanyMasterKey();
        }

        public static Guid GetCompanyMasterKey(this ClaimsPrincipal claimsPrincipal)
        {
            var claimValue = claimsPrincipal.FindFirst(CustomClaimTypes.CompanyMasterKey)?.Value;

            var parsedState = Guid.TryParse(claimValue, out var masterKey);

            if (!parsedState)
            {
                throw new InvalidOperationException($"Invalid Company Master Key");
            }

            return masterKey;
        }

        public static string GetUserLanguageIsoCode(this ControllerBase controllerBase)
        {
            return controllerBase.HttpContext.GetUserLanguageIsoCode();
        }

        public static string GetUserLanguageIsoCode(this HttpContext httpContext)
        {
            var languageIsoCode = string.Empty;

            var isClientCredentials = httpContext.User.IsClientCredentialsFlow();
            if (isClientCredentials)
            {
                var parse = httpContext.Request.Headers.TryGetValue(CustomClaimTypes.LanguageIsoCode, out StringValues languageIsoCodes);
                if (parse)
                {
                    languageIsoCode = languageIsoCodes.First();
                }
            }
            else
            {
                var claim = httpContext.User.FindFirst(CustomClaimTypes.LanguageIsoCode);

                if (claim == null)
                {
                    throw new InvalidOperationException($"{CustomClaimTypes.LanguageIsoCode} claim is missing");
                }

                languageIsoCode = claim.Value;
            }

            return languageIsoCode;
        }

        public static string GetUserFormatIsoCode(this ControllerBase controllerBase)
        {
            return controllerBase.HttpContext.GetUserFormatIsoCode();
        }

        public static string GetUserFormatIsoCode(this HttpContext httpContext)
        {
            var formatIsoCode = string.Empty;

            var isClientCredentials = httpContext.User.IsClientCredentialsFlow();
            if (isClientCredentials)
            {
                var parse = httpContext.Request.Headers.TryGetValue(CustomClaimTypes.FormatIsoCode, out StringValues languageIsoCodes);
                if (parse)
                {
                    formatIsoCode = languageIsoCodes.First();
                }
            }
            else
            {
                var claim = httpContext.User.FindFirst(CustomClaimTypes.FormatIsoCode);

                if (claim == null)
                {
                    throw new InvalidOperationException($"{CustomClaimTypes.FormatIsoCode} claim is missing");
                }

                formatIsoCode = claim.Value;
            }

            return formatIsoCode;
        }

        public static string GetUserTimeZone(this ControllerBase controllerBase)
        {
            return controllerBase.HttpContext.GetUserTimeZone();
        }

        public static string GetUserTimeZone(this HttpContext httpContext)
        {
            var timeZoneCode = string.Empty;

            var isClientCredentials = httpContext.User.IsClientCredentialsFlow();
            if (isClientCredentials)
            {
                var parse = httpContext.Request.Headers.TryGetValue(CustomClaimTypes.TimeZone, out StringValues headerValues);
                if (parse)
                {
                    timeZoneCode = headerValues.First();
                }
            }
            else
            {
                var claim = httpContext.User.FindFirst(CustomClaimTypes.TimeZone);

                if (claim == null)
                {
                    throw new InvalidOperationException($"{CustomClaimTypes.TimeZone} claim is missing");
                }

                timeZoneCode = claim.Value;
            }

            return timeZoneCode;
        }

        public static bool IsClientCredentialsFlow(this ClaimsPrincipal claimsPrincipal)
        {
            var carbonBasedUser = claimsPrincipal.HasClaim(x => x.Type == JwtClaimTypes.Subject);
            var clientId = claimsPrincipal.FindFirst(JwtClaimTypes.ClientId)?.Value;
            if (clientId == null)
            {
                throw new InvalidOperationException($"{JwtClaimTypes.ClientId} claim is missing");
            }

            if (!carbonBasedUser && clientId.Contains(nameof(GrantTypes.ClientCredentials), StringComparison.InvariantCulture))
            {
                return true;
            }

            return false;
        }

        public static bool HasCurrentModuleSecurityRight(this ControllerBase controllerBase, string rightCode)
        {
            return controllerBase.User.HasCurrentModuleSecurityRight(rightCode);
        }

        public static bool HasCurrentModuleSecurityRight(this ClaimsPrincipal claimsPrincipal, string rightCode)
        {
            var rights = claimsPrincipal.GetCurrentModuleSecurityRights();
            if (rights == null)
            {
                return false;
            }

            return rights.Any(x => x.SecurityRightCode == rightCode);
        }

        public static HashSet<SecurityRightIdentifierModel> GetCurrentModuleSecurityRights(this ClaimsPrincipal claimsPrincipal)
        {
            var rightsClaim = claimsPrincipal.FindFirst(CustomClaimTypes.SecurityRights)?.Value;

            if (rightsClaim == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<HashSet<SecurityRightIdentifierModel>>(rightsClaim);
        }

        public static async Task<string> GetTokenAsync(this ControllerBase controllerbase)
        {
            return await controllerbase.HttpContext.GetTokenAsync().ConfigureAwait(false);
        }

        public static async Task<string> GetTokenAsync(this HttpContext httpContext)
        {
            // Try to get the access token
            var token = await httpContext.GetTokenAsync(OidcConstants.TokenTypes.AccessToken).ConfigureAwait(false);

            if (token == null)
            {
                var tokenRetriever = TokenRetrieval.FromAuthorizationHeader();

                // Try to get token from Authorization header
                token = tokenRetriever(httpContext.Request);
            }

            return token;
        }
    }
}
