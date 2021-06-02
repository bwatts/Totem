using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Totem.Core;

namespace Totem.Local
{
    public class LocalQueryInfo : LocalMessageInfo
    {
        static readonly ConcurrentDictionary<Type, LocalQueryInfo> _infoByType = new();

        LocalQueryInfo(Type messageType, Type resultType) : base(messageType) =>
            ResultType = resultType;

        public Type ResultType { get; }

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out LocalQueryInfo info)
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

            var resultType = type.GetImplementedInterfaceGenericArguments(typeof(ILocalQuery<>)).SingleOrDefault();

            if(resultType != null)
            {
                info = new LocalQueryInfo(type, resultType);

                _infoByType[type] = info;

                return true;
            }

            return false;
        }

        public static bool TryFrom(ILocalQuery query, [MaybeNullWhen(false)] out LocalQueryInfo info)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            return TryFrom(query.GetType(), out info);
        }

        public static LocalQueryInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected query {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(ILocalQuery<>)}", nameof(type));

            return info;
        }

        public static LocalQueryInfo From(ILocalQuery query)
        {
            if(query == null)
                throw new ArgumentNullException(nameof(query));

            return From(query.GetType());
        }
    }
}