using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace BlazorApp1.Client.Auth
{
    public class JwtAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly HttpClient _httpClient;

        public JwtAuthenticationStateProvider(IJSRuntime jsRuntime, HttpClient httpClient)
        {
            _jsRuntime = jsRuntime;
            _httpClient = httpClient;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");

                if (string.IsNullOrWhiteSpace(token))
                {
                    return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);

                // For simplicity, we just extract basic info from the token. Usually token here is JWT.
                // Our backend returns token format: "{UsuarioId}:{Email}:{RolId}:{Nombre}" literally instead of JWT.
                // Let's parse it as claims.
                var parts = token.Split(':');
                if (parts.Length >= 4)
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, parts[0]),
                        new Claim(ClaimTypes.Name, parts[3]),
                        new Claim(ClaimTypes.Email, parts[1]),
                        new Claim(ClaimTypes.Role, parts[2]) // RolId as string
                    };
                    
                    var identity = new ClaimsIdentity(claims, "jwt");
                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }

                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            catch
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }

        public void NotifyUserAuthentication(string token)
        {
            var parts = token.Split(':');
            if (parts.Length >= 4)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, parts[0]),
                    new Claim(ClaimTypes.Name, parts[3]),
                    new Claim(ClaimTypes.Email, parts[1]),
                    new Claim(ClaimTypes.Role, parts[2])
                };
                var identity = new ClaimsIdentity(claims, "jwt");
                var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
                NotifyAuthenticationStateChanged(authState);
                
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            }
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
            NotifyAuthenticationStateChanged(authState);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }
}
