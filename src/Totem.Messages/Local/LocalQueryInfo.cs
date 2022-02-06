using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Totem.Core;

namespace Totem.Local;

public class LocalQueryInfo : QueryInfo, ILocalMessageInfo
{
    static readonly MessageInfoCache<LocalQueryInfo> _cache = new();

    LocalQueryInfo(Type declaredType, QueryResult result) : base(declaredType, result)
    { }

    public static bool TryFrom(Type type, [NotNullWhen(true)] out LocalQueryInfo? info)
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

        var resultType = type.GetImplementedInterfaceGenericArguments(typeof(ILocalQuery<>)).SingleOrDefault();

        if(resultType is not null && QueryResult.TryFrom(resultType, out var result))
        {
            info = new LocalQueryInfo(type, result);

            _cache.Add(info);

            return true;
        }

        return false;
    }

    public static LocalQueryInfo From(Type type)
    {
        if(!TryFrom(type, out var info))
            throw new ArgumentException($"Expected query {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(ILocalQuery<>)}", nameof(type));

        return info;
    }
}
