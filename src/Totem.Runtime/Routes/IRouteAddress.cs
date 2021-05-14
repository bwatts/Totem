using System;

namespace Totem.Routes
{
    public interface IRouteAddress
    {
        Type RouteType { get; }
        Id RouteId { get; }
    }
}