using System;

namespace Totem.Routes
{
    public class RouteAddress : IRouteAddress
    {
        public RouteAddress(Type routeType, Id routeId)
        {
            RouteType = routeType ?? throw new ArgumentNullException(nameof(routeType));
            RouteId = routeId ?? throw new ArgumentNullException(nameof(routeId));
        }

        public Type RouteType { get; }
        public Id RouteId { get; }

        public override string ToString() => $"{RouteType.Name}.{RouteId.ToCompactString()}";
    }
}