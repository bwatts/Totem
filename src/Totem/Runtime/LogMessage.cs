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
		public LogMessage(LogLevel level, Text template, params object[] propertyValues)
		{
			Level = level;
			Template = template;
			PropertyValues = propertyValues;
		}

		public LogMessage(LogLevel level, Exception error, Text template, params object[] propertyValues)
		{
			Level = level;
			Template = template;
			PropertyValues = propertyValues;
			Error = error;
		}

		public LogLevel Level { get; private set; }
		public Text Template { get; private set; }
		public object[] PropertyValues { get; private set; }
		public Exception Error { get; private set; }

		public override Text ToText()
		{
			return Template;
		}
	}
}