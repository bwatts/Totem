using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;

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

		public LogLevel Level { get; }

		public void Write(LogEvent e)
		{
      var level = GetEffectiveLevel(e);

      if(Logger.IsEnabled(level))
      {
        Write(e, level);
      }
		}

    LogEventLevel GetEffectiveLevel(LogEvent e)
    {
      var effectiveLevel = e.Level == LogLevel.Inherit ? Level : e.Level;

      return (LogEventLevel) (effectiveLevel - 1);
    }

    void Write(LogEvent e, LogEventLevel level)
		{
			if(e.Error != null)
			{
				Logger.Write(level, e.Error, e.Template, e.PropertyValues);
			}
			else
			{
				Logger.Write(level, e.Template, e.PropertyValues);
			}
		}
	}
}