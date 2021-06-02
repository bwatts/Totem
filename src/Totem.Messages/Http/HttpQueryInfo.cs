using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Totem.Core;

namespace Totem.Http
{
    public class HttpQueryInfo : HttpMessageInfo
    {
        static readonly ConcurrentDictionary<Type, HttpQueryInfo> _infoByType = new();

        HttpQueryInfo(Type messageType, HttpRequestInfo request, Type resultType) : base(messageType, request) =>
            ResultType = resultType;

        public Type ResultType { get; }

        public override string ToString() => $"{base.ToString()} = {ResultType}";

        public static bool IsHttpQuery(Type type) =>
            TryFrom(type, out var _);

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out HttpQueryInfo info)
        {
            if(_infoByType.TryGetValue(type, out info))
            {
                return true;
            }

            info = null;

            if(type == null || !type.IsConcreteClass())
            {
                return false;
            }

            var resultType = type.GetImplementedInterfaceGenericArguments(typeof(IHttpQuery<>)).SingleOrDefault();

            if(resultType != null && HttpRequestInfo.TryFrom(type, out var request))
            {
                info = new HttpQueryInfo(type, request, resultType);

                _infoByType[type] = info;

                return true;
            }

            return false;
        }

        public static bool TryFrom(IHttpQuery query, [MaybeNullWhen(false)] out HttpQueryInfo info)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            return TryFrom(query.GetType(), out info);
        }

        public static HttpQueryInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected query {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IHttpQuery<>)} and decorated with {nameof(GetRequestAttribute)} or {nameof(HeadRequestAttribute)}", nameof(type));

            return info;
        }

        public static HttpQueryInfo From(IHttpQuery query)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            return From(query.GetType());
        }
    }
}