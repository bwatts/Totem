using System.Diagnostics.CodeAnalysis;
using Totem.Core;

namespace Totem.Http;

public class HttpCommandInfo : CommandInfo, IHttpMessageInfo
{
    static readonly MessageInfoCache<HttpCommandInfo> _cache = new();

    HttpCommandInfo(Type declaredType, HttpRequestInfo request) : base(declaredType) =>
        Request = request;

    public HttpRequestInfo Request { get; }

    public override string ToString() =>
        $"{Request} => {DeclaredType}";

    public static bool TryFrom(Type type, [NotNullWhen(true)] out HttpCommandInfo? info)
    {
        if(type is null)
            throw new ArgumentNullException(nameof(type));

        if(_cache.TryGetValue(type, out info))
        {
            return true;
        }

        info = null;

        if(type is null || !type.IsConcreteClass())
        {
            return false;
        }

        if(typeof(IHttpCommand).IsAssignableFrom(type) && HttpRequestInfo.TryFrom(type, out var request))
        {
            info = new HttpCommandInfo(type, request);

            _cache.Add(info);

            return true;
        }

        return false;
    }

    public static HttpCommandInfo From(Type type)
    {
        if(!TryFrom(type, out var info))
            throw new ArgumentException($"Expected command {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IHttpCommand)} and decorated with {nameof(HttpPostRequestAttribute)}, {nameof(HttpPutRequestAttribute)}, or {nameof(HttpDeleteRequestAttribute)}", nameof(type));

        return info;
    }
}
