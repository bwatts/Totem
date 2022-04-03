using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace Totem.Hosting;

public class IncludeCredentialsHandler : DelegatingHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.SameOrigin);

        return base.SendAsync(request, cancellationToken);
    }
}
