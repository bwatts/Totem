using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Totem.Core;

namespace Totem
{
    public class QueryInfo
    {
        static readonly ConcurrentDictionary<Type, QueryInfo> _infosByType = new();

        QueryInfo(Type messageType, Type resultType, string method, string route)
        {
            MessageType = messageType;
            ResultType = resultType;
            Method = method;
            Route = route;
        }

        public Type MessageType { get; }
        public Type ResultType { get; }
        public string Method { get; }
        public string Route { get; }

        public static bool IsQuery(Type type) =>
            TryFrom(type, out var _);

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out QueryInfo info)
        {
            if(_infosByType.TryGetValue(type, out info))
            {
                return true;
            }

            info = null;

            if(type == null || !type.IsPublic || !type.IsClass || type.IsAbstract || type.ContainsGenericParameters)
            {
                return false;
            }

            var attribute = type.GetCustomAttribute<QueryAttribute>();

            if(attribute == null)
            {
                return false;
            }

            var resultType = type.GetImplementedInterfaceGenericArguments(typeof(IQuery<>)).SingleOrDefault();

            if(resultType != null)
            {
                info = new QueryInfo(type, resultType, attribute.Method, attribute.Route.Trim('/'));

                _infosByType[type] = info;

                return true;
            }

            return false;
        }

        public static bool TryFrom(IQuery query, [MaybeNullWhen(false)] out QueryInfo info)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            return TryFrom(query.GetType(), out info);
        }

        public static QueryInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected query {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IQuery<>)} and decorated with {nameof(GetRequestAttribute)} or {nameof(HeadRequestAttribute)}", nameof(type));

            return info;
        }

        public static QueryInfo From(IQuery query)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            return From(query.GetType());
        }
    }
}