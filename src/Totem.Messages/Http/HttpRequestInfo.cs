using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Totem.Core;

namespace Totem.Http;

public class HttpRequestInfo
{
    HttpRequestInfo(string method, string route)
    {
        Method = method;
        Route = route;
    }

    public string Method { get; }
    public string Route { get; }

    public static bool TryFrom(Type type, [NotNullWhen(true)] out HttpRequestInfo? info)
    {
        info = null;

        if(type is null || !type.IsConcreteClass())
        {
            return false;
        }

        var attribute = type.GetCustomAttribute<HttpRequestAttribute>();
        var method = attribute?.Method;
        var route = attribute?.Route.Trim('/');

        if(!string.IsNullOrWhiteSpace(method) && !string.IsNullOrWhiteSpace(route))
        {
            info = new HttpRequestInfo(method, route);
            return true;
        }

        return false;
    }

    public static bool TryFrom(IMessage message, [NotNullWhen(true)] out HttpRequestInfo? info)
    {
        if(message is null)
            throw new ArgumentNullException(nameof(message));

        return TryFrom(message.GetType(), out info);
    }

    public static HttpRequestInfo From(Type type)
    {
        if(!TryFrom(type, out var info))
            throw new ArgumentException($"Expected request {type} to be a public, non-abstract, non-or-closed-generic class decorated with {nameof(HttpRequestAttribute)} or a derived attribute", nameof(type));

        return info;
    }

    public static HttpRequestInfo From(IMessage message)
    {
        if(message is null)
            throw new ArgumentNullException(nameof(message));

        return From(message.GetType());
    }
}
