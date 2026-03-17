using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorApp1
{
    // This provider is used only during pre-rendering on the server.
    // Since JavaScript interop is not available during pre-rendering, we just return an unauthenticated user.
    // The WebAssembly client will take over authentication once the app becomes interactive.
    public class ServerAuthStateProvider : AuthenticationStateProvider
    {
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }
    }
}
