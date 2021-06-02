using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Totem.Core;

namespace Totem.Http
{
    public class HttpCommandInfo : HttpMessageInfo
    {
        static readonly ConcurrentDictionary<Type, HttpCommandInfo> _infoByType = new();

        HttpCommandInfo(Type messageType, HttpRequestInfo request) : base(messageType, request)
        { }

        public static bool IsHttpCommand(Type type) =>
            TryFrom(type, out var _);

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out HttpCommandInfo info)
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

            if(typeof(IHttpCommand).IsAssignableFrom(type) && HttpRequestInfo.TryFrom(type, out var request))
            {
                info = new HttpCommandInfo(type, request);

                _infoByType[type] = info;

                return true;
            }

            return false;
        }

        public static bool TryFrom(IHttpCommand command, [MaybeNullWhen(false)] out HttpCommandInfo info)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return TryFrom(command.GetType(), out info);
        }

        public static HttpCommandInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected command {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(IHttpCommand)} and decorated with {nameof(PostRequestAttribute)}, {nameof(PutRequestAttribute)}, or {nameof(DeleteRequestAttribute)}", nameof(type));

            return info;
        }

        public static HttpCommandInfo From(IHttpCommand command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return From(command.GetType());
        }
    }
}