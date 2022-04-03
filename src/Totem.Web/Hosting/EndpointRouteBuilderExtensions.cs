using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Totem.InternalApi;
using Totem.Subscriptions;

namespace Totem.Hosting;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapRuntimeHub(this IEndpointRouteBuilder endpoints)
    {
        if(endpoints is null)
            throw new ArgumentNullException(nameof(endpoints));

        endpoints.MapHub<SubscriptionHub>(InternalApiRoot.Combine("subscriptions"));

        return endpoints;
    }
}
