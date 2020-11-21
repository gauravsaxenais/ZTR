namespace ZTR.Framework.Security.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using EnsureThat;
    using IdentityModel;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;

    public class AuthorizeClaimFilter : IAsyncAuthorizationFilter
    {
        private readonly Claim _claim;
        private readonly ILogger<AuthorizeClaimFilter> _logger;
        private readonly SecurityOptions _securityOptions;
        private readonly IMemoryCache _memoryCache;
        private readonly double _cacheDurationSeconds = 300;

        private readonly IAuthorizeClient _authorizeClient;

        public AuthorizeClaimFilter(Claim claim, IAuthorizeClient authorizeClient, SecurityOptions securityOptions, IMemoryCache memoryCache, ILogger<AuthorizeClaimFilter> logger)
        {
            EnsureArg.IsNotNull(claim, nameof(claim));
            EnsureArg.IsNotNull(authorizeClient, nameof(authorizeClient));
            EnsureArg.IsNotNull(securityOptions, nameof(securityOptions));
            EnsureArg.IsNotNull(memoryCache, nameof(memoryCache));
            EnsureArg.IsNotNull(logger, nameof(logger));

            _claim = claim;
            _authorizeClient = authorizeClient;
            _securityOptions = securityOptions;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var claimsPrincipal = context.HttpContext.User;

            if (!claimsPrincipal.Identity.IsAuthenticated)
            {
                _logger.LogDebug($"User not Authenticated! Did you forget to decorate [Authorize] attribute");
            }

            if (claimsPrincipal.HasCurrentModuleSecurityRight(_claim.Value))
            {
                _logger.LogDebug($"Policy: {_claim.Value} was found in user claims. Action allowed");
            }
            else if (claimsPrincipal.IsClientCredentialsFlow())
            {
                // Client credential has all the rights on the claims. HasRightInClaims should return true.
                // If HasRightInClaims returns false then someone forgot to seed rights for an application module.
                var clientCredentials = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
                _logger.LogDebug($"Policy: {_claim.Value} was not found in client credential {clientCredentials} claims. Action denied");
                context.Result = new ForbidResult();
            }
            else
            {
                var allowed = await HasRightInAccessToken(context).ConfigureAwait(false);

                if (!allowed)
                {
                    context.Result = new ForbidResult();
                }
            }
        }

        private async Task<bool> HasRightInAccessToken(AuthorizationFilterContext context)
        {
            var policyName = _claim.Value;

            var token = await context.HttpContext.GetTokenAsync().ConfigureAwait(false);

            if (token == null)
            {
                _logger.LogDebug($"Policy: {policyName} token not found in request. Action denied");

                return false;
            }

            // TODO: implement cache
            IList<SecurityRightIdentifierModel> userRights = null; // = _memoryCache.Get<IList<SecurityRightIdentifierModel>>(token);

            if (userRights == null)
            {
                _logger.LogDebug($"Policy: {policyName} getting rights from user endpoint");

                _authorizeClient.AddCredentials(token);
                var rights = await _authorizeClient.GetUserRights(_securityOptions.ApplicationModuleCode).ConfigureAwait(false);
                userRights = rights.ToList();

                if (userRights != null)
                {
                    _memoryCache.Set(token, userRights, TimeSpan.FromSeconds(_cacheDurationSeconds));
                }
            }
            else
            {
                _logger.LogDebug($"Policy: {policyName} evaluating rights from cache. Cache duration is {_cacheDurationSeconds} seconds");
            }

            if (IsPolicyAllowed(userRights, policyName))
            {
                _logger.LogDebug($"Policy: {policyName} right found. Action allowed");

                return true;
            }

            _logger.LogDebug($"Policy: {policyName} right not found. Action denied");

            return false;
        }

        private bool IsPolicyAllowed(IList<SecurityRightIdentifierModel> userRights, string policyName)
        {
            return userRights != null && userRights.Any(p =>
                p.ApplicationModuleCode == _securityOptions.ApplicationModuleCode &&
                p.SecurityRightCode.Equals(policyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
