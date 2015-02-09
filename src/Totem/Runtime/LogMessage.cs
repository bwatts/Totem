using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// A signal to the log capturing execution details of the runtime
	/// </summary>
	public class LogMessage : Message
	{
		public LogMessage(Text description, object details = null, LogLevel level = LogLevel.Inherit, Terms terms = null, Exception error = null)
		{
			Description = description;
			Details = details;
			Level = level;
			Terms = terms ?? Terms.None;
			Error = error;
		}

		public Text Description { get; private set; }
		public object Details { get; private set; }
		public LogLevel Level { get; private set; }
		public Terms Terms { get; private set; }
		public Exception Error { get; private set; }
	}
}