
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Notes.Web.HttpHandlers
{
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public BearerTokenHandler(IHttpContextAccessor httpContextAccessor,
                   IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }


        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var accessToken = await GetAccessTokenAsync();

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.SetBearerToken(accessToken);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        public async Task<string> GetAccessTokenAsync()
        {
            try
            {

                return await _httpContextAccessor
                           .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

                // get the expires_at value & parse it
                var expiresAt = await _httpContextAccessor
                .HttpContext.GetTokenAsync("expires_at");

                var expiresAtAsDateTimeOffset =
                    DateTimeOffset.Parse(expiresAt, CultureInfo.InvariantCulture);

                if ((expiresAtAsDateTimeOffset.AddSeconds(-60)).ToUniversalTime() > DateTime.UtcNow)
                {
                    // no need to refresh, return the access token
                    return await _httpContextAccessor
                           .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
                }

                var idpClient = _httpClientFactory.CreateClient("IDPClient");

                // get the discovery document
                var discoveryReponse = await idpClient.GetDiscoveryDocumentAsync(_configuration["Idp:Address"] + _configuration["Idp:MetadataAddress"]);

                // refresh the tokens
                var refreshToken = await _httpContextAccessor
                           .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

                var refreshResponse = await idpClient.RequestRefreshTokenAsync(
                    new RefreshTokenRequest
                    {
                        Address = discoveryReponse.TokenEndpoint,
                        ClientId = _configuration["Idp:ClientId"],
                        ClientSecret = _configuration["Idp:ClientSecret"],
                        RefreshToken = refreshToken
                    });

                // store the tokens             
                var updatedTokens = new List<AuthenticationToken>();
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = refreshResponse.IdentityToken
                });
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = refreshResponse.AccessToken
                });
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = refreshResponse.RefreshToken
                });
                updatedTokens.Add(new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = (DateTime.UtcNow + TimeSpan.FromSeconds(refreshResponse.ExpiresIn)).
                            ToString("o", CultureInfo.InvariantCulture)
                });

                // get authenticate result, containing the current principal & 
                // properties
                var currentAuthenticateResult = await _httpContextAccessor
                    .HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // store the updated tokens
                currentAuthenticateResult.Properties.StoreTokens(updatedTokens);

                // sign in
                await _httpContextAccessor.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    currentAuthenticateResult.Principal,
                    currentAuthenticateResult.Properties);

                return refreshResponse.AccessToken;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
