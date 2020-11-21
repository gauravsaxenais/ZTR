namespace ZTR.Framework.Security
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using ZTR.Framework.Security.Authorization;
    using EnsureThat;
    using IdentityModel;
    using IdentityServer4;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using System.Linq;

    public static class AuthenticationExtension
    {
        // https://github.com/IdentityServer/IdentityServer4.AccessTokenValidation
        // https://tools.ietf.org/html/rfc7662
        public static IServiceCollection AddJwtBearerTokenAuthentication(this IServiceCollection services)
        {
            EnsureArg.IsNotNull(services, nameof(services));

            var securityOptions = services.BuildServiceProvider().GetRequiredService<SecurityOptions>();

            securityOptions.Validate();

            services
                .AddAuthentication(SecurityConstants.Bearer)
                .AddJwtBearer(SecurityConstants.Bearer, options =>
                {
                    options.SaveToken = true;
                    options.Authority = securityOptions.AuthorityEndpoint.OriginalString;
                    options.RequireHttpsMetadata = securityOptions.RequireHttpsMetadata;

                    options.Audience = securityOptions.TokenSecurityOptions.ApiCode;
                });

            return services;
        }

        public static IServiceCollection AddOpenIdConnectAndBearerTokenAuthentication(this IServiceCollection services, IServiceProvider serviceProvider = null)
        {
            EnsureArg.IsNotNull(services, nameof(services));

            if (serviceProvider == null)
            {
                serviceProvider = services.BuildServiceProvider();
            }

            var securityOptions = serviceProvider.GetRequiredService<SecurityOptions>();

            securityOptions.Validate();
            securityOptions.CookieSecurityOptions.Validate();

            //// TODO: implement the ticket store
            ////var httpClientHandler = new HttpClientHandler();

            ////// Adds the required platform services
            ////services.TryAddSingleton(httpClientHandler);

            ////var ticketStore = new TicketStore(
            ////       clientId: options.ClientId,
            ////       clientSecret: options.ClientSecret,
            ////       securityEndpointUrl: options.Authority.OriginalString,
            ////       httpClientHandler: httpClientHandler,
            ////       logger: loggerFactory.CreateLogger<TicketStore>(),
            ////       cacheDurationSeconds: options.CacheDurationSeconds);

            services
                .AddAuthentication(authenticationOptions =>
                {
                    authenticationOptions.DefaultScheme = SecurityConstants.CookieAuthenticationScheme;
                    authenticationOptions.DefaultChallengeScheme = SecurityConstants.Oidc;
                })
                .AddCookie(SecurityConstants.CookieAuthenticationScheme, settings =>
                {
                    settings.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    settings.Cookie.Name = securityOptions.CookieSecurityOptions.CookieName;
                    ////settings.SessionStore = ticketStore;
                })
                .AddOpenIdConnect(IdentityServerConstants.ProtocolTypes.OpenIdConnect, settings =>
                {
                    settings.Authority = securityOptions.AuthorityEndpoint.OriginalString;

                    settings.ClientId = securityOptions.CookieSecurityOptions.HybridClientId;
                    settings.ClientSecret = securityOptions.CookieSecurityOptions.HybridClientSecret.GuidToString();
                    settings.ResponseType = "code id_token";

                    settings.RequireHttpsMetadata = securityOptions.RequireHttpsMetadata;

                    settings.DisableTelemetry = true;

                    settings.GetClaimsFromUserInfoEndpoint = true;
                    settings.SaveTokens = true;

                    settings.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = JwtClaimTypes.Name,
                        RoleClaimType = JwtClaimTypes.Role,
                    };

                    // Defaults to /signout-callback-oidc; these are provided by OIDC middle ware.
                    ////settings.SignedOutCallbackPath
                    settings.SignedOutRedirectUri = securityOptions.CookieSecurityOptions.SignedOutRedirectUri.OriginalString;

                    settings.Scope.Clear();
                    foreach (var scope in securityOptions.CookieSecurityOptions.Scopes)
                    {
                        settings.Scope.Add(scope);
                    }

                    // Keep authentication method See: https://github.com/aspnet/Security/issues/1386
                    settings.ClaimActions.Remove(JwtClaimTypes.AuthenticationMethod);

                    settings.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.ClientId, JwtClaimTypes.ClientId);

                    settings.ClaimActions.MapUniqueJsonKey(CustomClaimTypes.CompanyMasterKey, CustomClaimTypes.CompanyMasterKey);
                    settings.ClaimActions.MapUniqueJsonKey(CustomClaimTypes.FormatIsoCode, CustomClaimTypes.FormatIsoCode);
                    settings.ClaimActions.MapUniqueJsonKey(CustomClaimTypes.LanguageIsoCode, CustomClaimTypes.LanguageIsoCode);
                    settings.ClaimActions.MapUniqueJsonKey(CustomClaimTypes.LoginProviderCode, CustomClaimTypes.LoginProviderCode);
                    settings.ClaimActions.MapUniqueJsonKey(JwtClaimTypes.PreferredUserName, JwtClaimTypes.PreferredUserName);
                    settings.ClaimActions.MapUniqueJsonKey(CustomClaimTypes.SecurityRights, CustomClaimTypes.SecurityRights);
                    settings.ClaimActions.MapUniqueJsonKey(CustomClaimTypes.TimeZone, CustomClaimTypes.TimeZone);

                    settings.Events = new OpenIdConnectEvents
                    {
                        OnRedirectToIdentityProvider = context =>
                        {
                            context.Request.QueryString = context.Request.QueryString.Add("theme", "theme");
                            context.ProtocolMessage.Parameters.Add("theme", "theme");

                            return Task.FromResult(0);
                        },
                        OnUserInformationReceived = context =>
                        {
                            return Task.FromResult(0);
                        },
                        OnRedirectToIdentityProviderForSignOut = context =>
                        {
                            var idTokenHint = context.Properties.GetTokenValue("id_token");

                            if (idTokenHint != null)
                            {
                                context.ProtocolMessage.IdTokenHint = idTokenHint;
                            }

                            return Task.FromResult(0);
                        },
                        OnRemoteFailure = context =>
                        {
                            if (context.Failure.HResult == -2146233088)
                            {
                                context.Response.Redirect(securityOptions.CookieSecurityOptions.SignedOutRedirectUri.OriginalString);
                                context.HandleResponse();
                            }

                            return Task.FromResult(0);
                        }
                    };
                })
                .AddBearerToken(securityOptions);

            return services;
        }

        public static IServiceCollection AddBearerTokenAuthentication(this IServiceCollection services, IServiceProvider serviceProvider = null)
        {
            EnsureArg.IsNotNull(services, nameof(services));
            ILogger logger;
            if (serviceProvider == null)
            {
                serviceProvider = services.BuildServiceProvider();
            }

            logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("AuthenticationExtension");
            logger.LogTrace("Starting AddBearerTokenAuthentication.");

            var securityOptions = serviceProvider.GetRequiredService<SecurityOptions>();

            securityOptions.Validate();
            securityOptions.TokenSecurityOptions.Validate();

            //// AuthenticationScheme = Bearer
            services
                .AddAuthentication(
                options =>
                {
                    options.DefaultScheme = SecurityConstants.Bearer;
                    options.DefaultChallengeScheme = SecurityConstants.Bearer;
                })
                .AddBearerToken(securityOptions);

            return services;
        }

        public static IServiceCollection AddClientCredentialsTokenAuthentication(this IServiceCollection services, IServiceProvider serviceProvider = null)
        {
            EnsureArg.IsNotNull(services, nameof(services));
            ILogger logger;
            if (serviceProvider == null)
            {
                serviceProvider = services.BuildServiceProvider();
            }

            logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(AuthenticationExtension));
            logger.LogTrace("Starting AddClientCredentialsTokenAuthentication.");

            var securityOptions = serviceProvider.GetRequiredService<SecurityOptions>();

            securityOptions.Validate();
            securityOptions.ClientCredentialsSecurityOptions.Validate();

            services.AddSingleton<ClientCredentialsTokens>();
            services.AddHttpClient();
            return services;
        }

        public static IServiceCollection AddClientCredentials<TService, TImplementation>(this IServiceCollection services, Uri serviceBaseUri)
            where TService : class, IClientCredentialsProvider
            where TImplementation : class, TService
        {
            return services.AddClientCredentials<TService, TImplementation>(serviceBaseUri.OriginalString);
        }

        public static IServiceCollection AddClientCredentials<TService, TImplementation>(this IServiceCollection services)
            where TService : class, IClientCredentialsProvider
            where TImplementation : class, TService
        {
            return services.AddClientCredentials<TService, TImplementation>(string.Empty);
        }

        public static IServiceCollection AddClientCredentialsForInternalWebServiceClient<TService, TImplementation>(this IServiceCollection services)
            where TService : class, IClientCredentialsProvider
            where TImplementation : class, TService
        {
            return services.AddClientCredentialsForInternalWebServiceClient<TService, TImplementation>(string.Empty);
        }

        [Obsolete("Keeping this as a backup")]
        public static IServiceCollection AddBearerTokenAuthorization(this IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.Configure<AuthorizationOptions>(opt => { });

            services.AddSingleton<IAuthorizationPolicyProvider>(srv =>
            {
                var securityOptions = srv.GetRequiredService<SecurityOptions>();

                var authorizationPolicyProvider =
                   new AuthorizationPolicyProvider(
                   srv.GetRequiredService<IOptions<AuthorizationOptions>>(),
                   srv.GetRequiredService<IHttpContextAccessor>(),
                   srv.GetRequiredService<IAuthorizeClient>(),
                   srv.GetRequiredService<SecurityOptions>(),
                   srv.GetRequiredService<IMemoryCache>(),
                   srv.GetRequiredService<ILogger<AuthorizationPolicyProvider>>());

                return authorizationPolicyProvider;
            });

            return services;
        }

        internal static void Validate(this SecurityOptions securityOptions)
        {
            EnsureArg.IsNotNull(securityOptions, nameof(securityOptions));
            EnsureArg.IsNotNull(securityOptions.ApplicationModuleCode, nameof(securityOptions.ApplicationModuleCode));
            EnsureArg.IsNotNull(securityOptions.AuthorityEndpoint, nameof(securityOptions.AuthorityEndpoint));
            EnsureArg.IsGte(securityOptions.CacheDurationSeconds, 1000, nameof(securityOptions.CacheDurationSeconds));

            var nullConfiguration = securityOptions.CookieSecurityOptions == null && securityOptions.TokenSecurityOptions == null && securityOptions.ClientCredentialsSecurityOptions == null;
            EnsureArg.IsTrue(!nullConfiguration, nameof(nullConfiguration), x => x.WithMessage("Configure at least one security option. (cookie | token | client credentials)"));
        }

        internal static void Validate(this ClientCredentialsSecurityOptions clientCredentialsSecurityOptions)
        {
            EnsureArg.IsNotNull(clientCredentialsSecurityOptions, nameof(clientCredentialsSecurityOptions));
            EnsureArg.IsNotNullOrWhiteSpace(clientCredentialsSecurityOptions.CredentialsClientId, nameof(clientCredentialsSecurityOptions.CredentialsClientId));
            EnsureArg.IsNotEmpty(clientCredentialsSecurityOptions.CredentialsClientSecret, nameof(clientCredentialsSecurityOptions.CredentialsClientSecret));
            EnsureArg.HasItems(clientCredentialsSecurityOptions.Scopes.ToList(), nameof(clientCredentialsSecurityOptions.Scopes));
        }

        private static void Validate(this CookieSecurityOptions cookieSecurityOptions)
        {
            EnsureArg.IsNotNull(cookieSecurityOptions, nameof(cookieSecurityOptions));
            EnsureArg.IsNotNullOrWhiteSpace(cookieSecurityOptions.HybridClientId, nameof(cookieSecurityOptions.HybridClientId));
            EnsureArg.IsNotEmpty(cookieSecurityOptions.HybridClientSecret, nameof(cookieSecurityOptions.HybridClientSecret));
            EnsureArg.IsNotNullOrWhiteSpace(cookieSecurityOptions.CookieName, nameof(cookieSecurityOptions.CookieName));
            EnsureArg.IsNotNull(cookieSecurityOptions.SignedOutRedirectUri, nameof(cookieSecurityOptions.SignedOutRedirectUri));
            EnsureArg.HasItems(cookieSecurityOptions.Scopes.ToList(), nameof(cookieSecurityOptions.Scopes));
        }

        private static void Validate(this TokenSecurityOptions tokenSecurityOptions)
        {
            EnsureArg.IsNotNull(tokenSecurityOptions, nameof(tokenSecurityOptions));
            EnsureArg.IsNotEmpty(tokenSecurityOptions.ApiCode, nameof(tokenSecurityOptions.ApiCode));
            EnsureArg.IsNotEmpty(tokenSecurityOptions.ApiSecret, nameof(tokenSecurityOptions.ApiSecret));
            EnsureArg.HasItems(tokenSecurityOptions.Scopes.ToList(), nameof(tokenSecurityOptions.Scopes));
            
            if (!(string.IsNullOrWhiteSpace(tokenSecurityOptions.SwaggerClientId) == (tokenSecurityOptions.SwaggerClientSecret == Guid.Empty)))
            {
                var paramName = $"{nameof(tokenSecurityOptions.SwaggerClientId)} {nameof(tokenSecurityOptions.SwaggerClientSecret)}";
                throw new ArgumentNullException(paramName, $"{nameof(tokenSecurityOptions.SwaggerClientId)} and {nameof(tokenSecurityOptions.SwaggerClientSecret)} both must be provided or both must be empty");
            }
        }

        private static AuthenticationBuilder AddBearerToken(this AuthenticationBuilder builder, SecurityOptions securityOptions)
        {
            return builder
                .AddIdentityServerAuthentication(
                OidcConstants.AuthenticationSchemes.AuthorizationHeaderBearer,
                settings =>
                {
                    settings.Authority = securityOptions.AuthorityEndpoint.OriginalString;
                    settings.ApiName = securityOptions.TokenSecurityOptions.ApiCode;
                    settings.ApiSecret = securityOptions.TokenSecurityOptions.ApiSecret.GuidToString();
                    settings.CacheDuration = TimeSpan.FromSeconds(securityOptions.CacheDurationSeconds);
                    settings.EnableCaching = true;
                    settings.RequireHttpsMetadata = securityOptions.RequireHttpsMetadata;
                    settings.SaveToken = true;
                });
        }

        private static IServiceCollection AddClientCredentials<TService, TImplementation>(this IServiceCollection services, string serviceBaseUri)
            where TService : class, IClientCredentialsProvider
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>(
                (serviceProvider) =>
                {
                    var userClientInstance = (TImplementation)Activator.CreateInstance(typeof(TImplementation), serviceBaseUri, new HttpClient());
                    return AddClientCredentials<TService, TImplementation>(serviceProvider, userClientInstance);
                });

            return services;
        }

        private static TImplementation AddClientCredentials<TService, TImplementation>(IServiceProvider serviceProvider, TImplementation userClientInstance)
            where TService : class, IClientCredentialsProvider
            where TImplementation : class, TService
        {
            var clientCredentialsTokens = serviceProvider.GetRequiredService<ClientCredentialsTokens>();

            userClientInstance.AddCredentials(clientCredentialsTokens);
            
            return userClientInstance;
        }

        private static IServiceCollection AddClientCredentialsForInternalWebServiceClient<TService, TImplementation>(this IServiceCollection services, string serviceBaseUri)
            where TService : class, IClientCredentialsProvider
            where TImplementation : class, TService
        {
            services.AddTransient<TService, TImplementation>(
                serviceProvider =>
                {
                    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                    var userClientInstance = (TImplementation)Activator.CreateInstance(typeof(TImplementation), serviceBaseUri, httpClientFactory);
                    return AddClientCredentials<TService, TImplementation>(serviceProvider, userClientInstance);
                });

            return services;
        }
    }
}
