using System;

namespace Totem.Timelines
{
    public class UnexpectedVersionException : Exception
    {
        public UnexpectedVersionException(string? message, Exception? inner = null) : base(message, inner)
        { }
    }
}