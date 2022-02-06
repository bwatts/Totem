using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Totem.Core;

namespace Totem.Http;

public class HttpQueryInfo : QueryInfo, IHttpMessageInfo
{
    static readonly MessageInfoCache<HttpQueryInfo> _cache = new();

    HttpQueryInfo(Type declaredType, QueryResult result, HttpRequestInfo request) : base(declaredType, result) =>
        Request = request;

    public HttpRequestInfo Request { get; }

    public override string ToString() =>
        $"{DeclaredType} => {Result}";

    public static bool TryFrom(Type type, [NotNullWhen(true)] out HttpQueryInfo? info)
    {
        if(_cache.TryGetValue(type, out info))
        {
            return true;
        }

        info = null;

        if(type is null || !type.IsConcreteClass())
        {
            return false;
        }

        var resultType = type.GetImplementedInterfaceGenericArguments(typeof(IHttpQuery<>)).SingleOrDefault();

        if(resultType is not null
            && QueryResult.TryFrom(resultType, out var result)
            && HttpRequestInfo.TryFrom(type, out var request))
        {
            info = new HttpQueryInfo(type, result, request);

            _cache.Add(info);

            return true;
        }

        return false;
    }

    public static bool TryFrom(IHttpQuery query, [NotNullWhen(true)] out HttpQueryInfo? info)
    {
        if(query is null)
            throw new ArgumentNullException(nameof(query));

        return TryFrom(query.GetType(), out info);
    }

    public static HttpQueryInfo From(Type type)
    {
        if(!TryFrom(type, out var info))
            throw new ArgumentException($"Expected type to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IHttpQuery<>)} and decorated with {nameof(HttpGetRequestAttribute)} or {nameof(HttpHeadRequestAttribute)}: {type}", nameof(type));

        return info;
    }

    public static HttpQueryInfo From(IHttpQuery query)
    {
        if(query is null)
            throw new ArgumentNullException(nameof(query));

        return From(query.GetType());
    }
}
