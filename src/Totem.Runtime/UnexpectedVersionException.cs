using System;

namespace Totem
{
    public class UnexpectedVersionException : Exception
    {
        public UnexpectedVersionException(string? message, Exception? inner = null) : base(message, inner)
        { }
    }
}