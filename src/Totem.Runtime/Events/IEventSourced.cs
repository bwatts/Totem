using System;
using System.Collections.Generic;

namespace Totem.Events
{
    public interface IEventSourced
    {
        IEnumerable<Type> GivenTypes { get; }
        long? Version { get; }
        bool HasErrors { get; }
        IEnumerable<ErrorInfo> Errors { get; }

        void Load(IEvent given, long? version = null);
    }
}