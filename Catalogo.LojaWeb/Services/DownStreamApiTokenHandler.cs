using Microsoft.AspNetCore.Authentication;
using System.Net.Http.Headers;

namespace Catalogo.LojaWeb.Services
{
    public class DownStreamApiTokenHandler(IHttpContextAccessor httpContextAccessor) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            HttpContext? context = httpContextAccessor.HttpContext;
            if (context is not null)
            {
                string? accessToken = await context.GetTokenAsync("access_token");
                if (accessToken is not null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                }
            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
