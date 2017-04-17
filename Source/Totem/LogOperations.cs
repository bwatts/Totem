using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Totem.Runtime;

namespace Totem
{
  /// <summary>
  /// Extends <see cref="T:Totem.Runtime.ILog"/> with writes for specific scenarios
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
	public static class LogOperations
	{
		//
		// Entries
		//

		public static void At(this ILog log, LogLevel level, Text messageTemplate, params object[] propertyValues)
		{
      log.Write(new LogEvent(level, messageTemplate, propertyValues));
    }

    public static void At(this ILog log, LogLevel level, Exception error, Text messageTemplate, params object[] propertyValues)
		{
      log.Write(new LogEvent(error.ToString(), level, messageTemplate, propertyValues));
    }

    public static void Verbose(this ILog log, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Verbose, messageTemplate, propertyValues);
		}

		public static void Verbose(this ILog log, Exception error, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Verbose, error, messageTemplate, propertyValues);
		}

		public static void Debug(this ILog log, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Debug, messageTemplate, propertyValues);
		}

		public static void Debug(this ILog log, Exception error, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Debug, error, messageTemplate, propertyValues);
		}

		public static void Info(this ILog log, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Info, messageTemplate, propertyValues);
		}

		public static void Info(this ILog log, Exception error, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Info, error, messageTemplate, propertyValues);
		}

		public static void Warning(this ILog log, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Warning, messageTemplate, propertyValues);
		}

		public static void Warning(this ILog log, Exception error, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Warning, error, messageTemplate, propertyValues);
		}

		public static void Error(this ILog log, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Error, messageTemplate, propertyValues);
		}

		public static void Error(this ILog log, Exception error, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Error, error, messageTemplate, propertyValues);
		}

		public static void Fatal(this ILog log, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Fatal, messageTemplate, propertyValues);
		}

		public static void Fatal(this ILog log, Exception error, Text messageTemplate, params object[] propertyValues)
		{
			log.At(LogLevel.Fatal, error, messageTemplate, propertyValues);
		}

		//
		// CanWrite
		//

		public static bool IsAt(this ILog log, LogLevel level)
		{
			return level == LogLevel.Inherit || log.Level == LogLevel.Inherit || level >= log.Level;
		}

		public static bool IsAtVerbose(this ILog log)
		{
			return log.IsAt(LogLevel.Verbose);
		}

		public static bool IsAtDebug(this ILog log)
		{
			return log.IsAt(LogLevel.Debug);
		}

		public static bool IsAtInfo(this ILog log)
		{
			return log.IsAt(LogLevel.Info);
		}

		public static bool IsAtWarning(this ILog log)
		{
			return log.IsAt(LogLevel.Warning);
		}

		public static bool IsAtError(this ILog log)
		{
			return log.IsAt(LogLevel.Error);
		}

		public static bool IsAtFatal(this ILog log)
		{
			return log.IsAt(LogLevel.Fatal);
		}
	}
}