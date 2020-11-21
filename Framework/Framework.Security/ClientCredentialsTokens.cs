namespace ZTR.Framework.Security
{
    using System;
    using System.Globalization;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using EnsureThat;
    using IdentityModel.Client;
    using Microsoft.Extensions.Logging;

    public class ClientCredentialsTokens : IDisposable
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly SecurityOptions _securityOptions;
        private readonly ILogger<ClientCredentialsTokens> _logger;
        private readonly DiscoveryPolicy _discoveryClientPolicy;

        private bool _disposed;
        private TokenResponse _tokenResponse;
        private DateTimeOffset _tokenExpiresDate = DateTimeOffset.Now;

        public ClientCredentialsTokens(SecurityOptions securityoptions, ILogger<ClientCredentialsTokens> logger)
        {
            EnsureArg.IsNotNull(securityoptions, nameof(securityoptions));
            EnsureArg.IsNotNull(logger, nameof(logger));

            securityoptions.Validate();
            securityoptions.ClientCredentialsSecurityOptions.Validate();

            _securityOptions = securityoptions;
            _logger = logger;
            _discoveryClientPolicy = new DiscoveryPolicy { RequireHttps = securityoptions.RequireHttpsMetadata };
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (DateTimeOffset.Now > _tokenExpiresDate || _tokenResponse == null)
            {
                await _semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    if (DateTimeOffset.Now > _tokenExpiresDate || _tokenResponse == null)
                    {
                        _logger.LogInformation("The client credentials token expired. Getting a new one from the authority.");
                        _tokenResponse = await GetNewToken().ConfigureAwait(false);

                        var originalExpirationDate = DateTimeOffset.Now.AddSeconds(_tokenResponse.ExpiresIn);

                        // Add a minute to ensure we have some buffer time to complete before expiring.
                        _tokenExpiresDate = originalExpirationDate.AddMinutes(-1);
                        _logger.LogInformation(
                            $"Got a new token from the authority. The token expiration date is {originalExpirationDate.ToString("o", CultureInfo.InvariantCulture)}."
                            + $" The token will be renewed at {_tokenExpiresDate.ToString("o", CultureInfo.InvariantCulture)}.");
                    }
                }
                finally
                {
                    _semaphore.Release();
                }
            }

            // Not locking because we have a buffer time.
            return _tokenResponse.AccessToken;
        }

        public void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Free any other managed objects here.
                _disposed = true;

                _semaphore.Wait();
                _tokenResponse = null;
                _semaphore.Dispose();
            }

            _disposed = true;
        }

        private async Task<TokenResponse> GetNewToken()
        {
            var client = new HttpClient();

            var authorityUrl = _securityOptions.AuthorityEndpoint.OriginalString;

            var discoveryDocumentRequest = new DiscoveryDocumentRequest()
            {
                Address = authorityUrl,
                Policy = _discoveryClientPolicy
            };

            var discoveryResponse = await client.GetDiscoveryDocumentAsync(discoveryDocumentRequest).ConfigureAwait(false);

            if (discoveryResponse.IsError)
            {
                _logger.LogError($"Authority discovery error. Authority: {authorityUrl}. Error: {discoveryResponse.Error}");

                if (discoveryResponse.Exception != null)
                {
                    throw discoveryResponse.Exception;
                }

                throw new InvalidOperationException(discoveryResponse.Error);
            }

            var tokenEndPointUrl = discoveryResponse.TokenEndpoint;
            _logger.LogDebug($"Authority token end point: {tokenEndPointUrl}.");

            var clientCredentialsTokenRequest = new ClientCredentialsTokenRequest()
            {
                Address = tokenEndPointUrl,
                ClientId = _securityOptions.ClientCredentialsSecurityOptions.CredentialsClientId,
                ClientSecret = _securityOptions.ClientCredentialsSecurityOptions.CredentialsClientSecret.GuidToString(),
                Scope = string.Join(" ", _securityOptions.ClientCredentialsSecurityOptions.Scopes),
            };

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(clientCredentialsTokenRequest).ConfigureAwait(false);

            if (tokenResponse.IsError)
            {
                _logger.LogError($"Error RequestClientCredentialsToken: {tokenResponse.Error}.");

                throw new InvalidOperationException(tokenResponse.Error);
            }

            return tokenResponse;
        }
    }
}
