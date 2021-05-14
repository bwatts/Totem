using System;

namespace Totem.Core
{
    public interface IQueryEnvelope : IMessageEnvelope
    {
        new IQuery Message { get; }
        Type ResultType { get; }
    }
}