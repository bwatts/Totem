using System;

namespace Totem
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class CommandAttribute : Attribute
    {
        internal CommandAttribute(string method, string route)
        {
            Method = !string.IsNullOrWhiteSpace(method) ? method : throw new ArgumentOutOfRangeException(nameof(method));
            Route = !string.IsNullOrWhiteSpace(route) ? route : throw new ArgumentOutOfRangeException(nameof(route));
        }

        public string Method { get; }
        public string Route { get; }

        public override string ToString() => $"{Method} {Route}";
    }
}