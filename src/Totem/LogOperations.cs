using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime;

namespace Totem
{
	/// <summary>
	/// Extends <see cref="T:Totem.Runtime.ILog"/> with writes for specific scenarios
	/// </summary>
	public static class LogOperations
	{
		//
		// Entries
		//

		public static void Write(this ILog log, Text description, object details = null, LogLevel level = LogLevel.Inherit, Terms terms = null, Exception error = null)
		{
			log.Write(new LogMessage(description, details, level, terms, error));
		}

		public static void Verbose(this ILog log, Text description, object details = null, Terms terms = null, Exception error = null)
		{
			log.Write(description, details, LogLevel.Verbose, terms, error);
		}

		public static void Debug(this ILog log, Text description, object details = null, Terms terms = null, Exception error = null)
		{
			log.Write(description, details, LogLevel.Debug, terms, error);
		}

		public static void Info(this ILog log, Text description, object details = null, Terms terms = null, Exception error = null)
		{
			log.Write(description, details, LogLevel.Info, terms, error);
		}

		public static void Warning(this ILog log, Text description, object details = null, Terms terms = null, Exception error = null)
		{
			log.Write(description, details, LogLevel.Warning, terms, error);
		}

		public static void Error(this ILog log, Text description, object details = null, Terms terms = null, Exception error = null)
		{
			log.Write(description, details, LogLevel.Error, terms, error);
		}

		public static void Fatal(this ILog log, Text description, object details = null, Terms terms = null, Exception error = null)
		{
			log.Write(description, details, LogLevel.Fatal, terms, error);
		}

		//
		// CanWrite
		//

		public static bool CanWrite(this ILog log, LogLevel level)
		{
			return level == LogLevel.Inherit || log.Level == LogLevel.Inherit || level >= log.Level;
		}

		public static bool CanWriteVerbose(this ILog log)
		{
			return log.CanWrite(LogLevel.Verbose);
		}

		public static bool CanWriteDebug(this ILog log)
		{
			return log.CanWrite(LogLevel.Debug);
		}

		public static bool CanWriteInfo(this ILog log)
		{
			return log.CanWrite(LogLevel.Info);
		}

		public static bool CanWriteWarning(this ILog log)
		{
			return log.CanWrite(LogLevel.Warning);
		}

		public static bool CanWriteError(this ILog log)
		{
			return log.CanWrite(LogLevel.Error);
		}

		public static bool CanWriteFatal(this ILog log)
		{
			return log.CanWrite(LogLevel.Fatal);
		}
	}
}