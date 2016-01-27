using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Totem
{
	/// <summary>
	/// Indicates a value failed to meet an expectation
	/// </summary>
	[Serializable]
	public class ExpectException : Exception
	{
		private readonly Lazy<string> _stackTrace;

		internal ExpectException(string message) : base(message)
		{
			_stackTrace = new Lazy<string>(FilterStackTrace);
		}

		internal ExpectException(string message, Exception inner) : base(message, inner)
		{
			_stackTrace = new Lazy<string>(FilterStackTrace);
		}

    protected ExpectException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
			_stackTrace = new Lazy<string>(FilterStackTrace);
		}

		public override string StackTrace => _stackTrace.Value;

		private string FilterStackTrace()
		{
			return Text
				.Of(base.StackTrace)
				.SplitLines()
				.Where(line => !line.Contains("at Totem.Expect`1.") && !line.Contains("at Totem.Expectable."))
				.ToTextSeparatedBy(Text.Line);
		}
	}
}