using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace Totem
{
	/// <summary>
	/// Indicates a spec with an unexpected outcome
	/// </summary>
	public class SpecException : AssertException
	{
		internal SpecException(string message, Exception inner) : base(message, inner)
		{}
	}
}