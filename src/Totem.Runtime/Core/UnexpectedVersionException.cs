using System;

namespace Totem.Core;

public class UnexpectedVersionException : Exception
{
    public UnexpectedVersionException(string? message, Exception? inner = null) : base(message, inner)
    { }
}
