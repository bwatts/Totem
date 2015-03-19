using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

namespace Totem.Runtime.Configuration
{
	/// <summary>
	/// Writes log messages to a Serilog logger
	/// </summary>
	internal sealed class SerilogAdapter : ILog
	{
		internal SerilogAdapter(ILogger logger, LogLevel level)
		{
			Logger = logger;
			Level = level;
		}

		internal readonly ILogger Logger;

		public LogLevel Level { get; private set; }

		public void Write(LogMessage message)
		{
			var effectiveLevel = message.Level == LogLevel.Inherit ? Level : message.Level;

			var level = (LogEventLevel) (effectiveLevel - 1);

			if(Logger.IsEnabled(level))
			{
				Write(message, level);
			}
		}

		private void Write(LogMessage message, LogEventLevel level)
		{
			if(message.Error != null)
			{
				Logger.Write(level, message.Error, message.Template, message.PropertyValues);
			}
			else
			{
				Logger.Write(level, message.Template, message.PropertyValues);
			}
		}
	}
}