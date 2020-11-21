namespace ZTR.Framework.Security
{
    using System.Threading.Tasks;
    using EnsureThat;
    using IdentityModel;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.Extensions.Logging;

    public class ClientCredentialOnlyFilter : IAsyncActionFilter
    {
        private readonly ILogger<ClientCredentialOnlyFilter> _logger;

        public ClientCredentialOnlyFilter(ILogger<ClientCredentialOnlyFilter> logger)
        {
            EnsureArg.IsNotNull(logger, nameof(logger));
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claimsPrincipal = context.HttpContext.User;
            if (!claimsPrincipal.IsClientCredentialsFlow())
            {
                var carbonBasedUser = claimsPrincipal.FindFirst(JwtClaimTypes.Name)?.Value;
                _logger.LogDebug($"method restricted to client credentials only. for user {carbonBasedUser}");
                context.Result = new ForbidResult();
            }
            else
            {
                await next().ConfigureAwait(false);
            }
        }
    }
}
