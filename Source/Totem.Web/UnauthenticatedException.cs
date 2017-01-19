using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Web
{
	/// <summary>
	/// Indicates a request could not be authenticated
	/// </summary>
	public class UnauthenticatedException : Exception
	{
    public UnauthenticatedException()
    {}

    public UnauthenticatedException(string message) : base(message)
		{}

    public UnauthenticatedException(string message, Exception inner) : base(message, inner)
		{}
	}
}