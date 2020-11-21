namespace ZTR.Framework.Security.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using EnsureThat;
    using IdentityModel.AspNetCore.OAuth2Introspection;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using static IdentityModel.OidcConstants;

    [Obsolete("Keeping this as a backup")]
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly IAuthorizeClient _authorizeClient;
        private readonly double _cacheDurationSeconds;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly SecurityOptions _securityOptions;
        private readonly ILogger<AuthorizationPolicyProvider> _logger;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options, IHttpContextAccessor contextAccessor, IAuthorizeClient authorizeClient, SecurityOptions securityOptions, IMemoryCache memoryCache, ILogger<AuthorizationPolicyProvider> logger, double cacheDurationSeconds = 300)
            : base(options)
        {
            EnsureArg.IsNotNull(options, nameof(options));
            EnsureArg.IsNotNull(authorizeClient, nameof(authorizeClient));
            EnsureArg.IsGt(300, cacheDurationSeconds, nameof(cacheDurationSeconds));
            EnsureArg.IsNotNull(contextAccessor, nameof(contextAccessor));
            EnsureArg.IsNotNull(memoryCache, nameof(memoryCache));
            EnsureArg.IsNotNull(securityOptions, nameof(securityOptions));
            EnsureArg.IsNotNull(logger, nameof(logger));

            _authorizeClient = authorizeClient;
            _cacheDurationSeconds = cacheDurationSeconds;
            _contextAccessor = contextAccessor;
            _memoryCache = memoryCache;
            _securityOptions = securityOptions;
            _logger = logger;

            Allowed = new AuthorizationPolicyBuilder(SecurityConstants.Bearer).RequireAuthenticatedUser().Build();
            Denied = new AuthorizationPolicyBuilder(SecurityConstants.Bearer).RequireAssertion(p => { return false; }).Build();
        }

        public AuthorizationPolicy Allowed { get; private set; }

        public AuthorizationPolicy Denied { get; private set; }

        public async override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            return await ValidatePolicy(policyName).ConfigureAwait(false);
        }

        private async Task<AuthorizationPolicy> ValidatePolicy(string policyName)
        {
            var context = _contextAccessor.HttpContext;

            // Try to get the access token
            var token = await context.GetTokenAsync(TokenTypes.AccessToken).ConfigureAwait(false);

            if (token == null)
            {
                var tokenRetriever = TokenRetrieval.FromAuthorizationHeader();

                // Try to get token from Authorization header
                token = tokenRetriever(context.Request);
            }

            if (token == null)
            {
                _logger.LogDebug($"Policy: {policyName} token not found in request. Action denied");

                return Denied;
            }

            var authenticateResult = await _contextAccessor.HttpContext.AuthenticateAsync().ConfigureAwait(false);

            if (!authenticateResult.Succeeded)
            {
                _logger.LogDebug($"Policy: token authentication failed {authenticateResult.Failure.Message}. Action denied");

                return Denied;
            }

            if (HasRightInClaims(policyName, authenticateResult.Principal))
            {
                _logger.LogDebug($"Policy: {policyName} was found in user claims. Action allowed");
                return Allowed;
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

                return Allowed;
            }

            _logger.LogDebug($"Policy: {policyName} right not found. Action denied");

            return Denied;
        }

        private bool HasRightInClaims(string policyName, ClaimsPrincipal claimsPrincipal)
        {
            var rightsClaim = claimsPrincipal.FindFirst(CustomClaimTypes.SecurityRights)?.Value;

            if (rightsClaim == null)
            {
                return false;
            }

            var userRights = JsonConvert.DeserializeObject<IList<Security.SecurityRightIdentifierModel>>(rightsClaim);
            return IsPolicyAllowed(userRights, policyName);
        }

        private bool IsPolicyAllowed(IList<SecurityRightIdentifierModel> userRights, string policyName)
        {
            return userRights != null && userRights.Any(p =>
                p.ApplicationModuleCode == _securityOptions.ApplicationModuleCode &&
                p.SecurityRightCode.Equals(policyName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
