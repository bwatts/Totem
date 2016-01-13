using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Totem
{
	/// <summary>
	/// Indicates a value failed to meet an expectation
	/// </summary>
	/// <remarks>
	/// http://stackoverflow.com/questions/94488/what-is-the-correct-way-to-make-a-custom-net-exception-serializable
	/// </remarks>
	[Serializable]
	public class ExpectException : Exception
	{
		internal ExpectException(string message) : base(message)
		{}

		internal ExpectException(string message, Exception inner) : base(message, inner)
		{}

    protected ExpectException(SerializationInfo info, StreamingContext context) : base(info, context)
    {}
	}
}