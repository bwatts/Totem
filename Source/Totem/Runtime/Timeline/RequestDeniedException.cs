using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Indicates a request denied access to its client
	/// </summary>
	public class RequestDeniedException : Exception
	{
    public RequestDeniedException()
    {}

    public RequestDeniedException(string message) : base(message)
		{}

    public RequestDeniedException(string message, Exception inner) : base(message, inner)
		{}
	}
}