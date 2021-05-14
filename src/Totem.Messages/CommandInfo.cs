using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Totem
{
    public class CommandInfo
    {
        static readonly ConcurrentDictionary<Type, CommandInfo> _infosByType = new();

        CommandInfo(Type messageType, string method, string route)
        {
            MessageType = messageType;
            Method = method;
            Route = route;
        }

        public Type MessageType { get; }
        public string Method { get; }
        public string Route { get; }

        public static bool IsCommand(Type type) =>
            TryFrom(type, out var _);

        public static bool TryFrom(Type type, [MaybeNullWhen(false)] out CommandInfo info)
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

            var attribute = type.GetCustomAttributes().OfType<CommandAttribute>().SingleOrDefault();

            if(attribute == null)
            {
                return false;
            }

            if(typeof(ICommand).IsAssignableFrom(type))
            {
                info = new CommandInfo(type, attribute.Method, attribute.Route.Trim('/'));

                _infosByType[type] = info;

                return true;
            }

            return false;
        }

        public static bool TryFrom(ICommand command, [MaybeNullWhen(false)] out CommandInfo info)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return TryFrom(command.GetType(), out info);
        }

        public static CommandInfo From(Type type)
        {
            if(!TryFrom(type, out var info))
                throw new ArgumentException($"Expected command {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(ICommand)} and decorated with {nameof(PostRequestAttribute)}, {nameof(PutRequestAttribute)}, or {nameof(DeleteRequestAttribute)}", nameof(type));

            return info;
        }

        public static CommandInfo From(ICommand command)
        {
            if(command == null)
                throw new ArgumentNullException(nameof(command));

            return From(command.GetType());
        }
    }
}