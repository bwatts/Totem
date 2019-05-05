using System;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace Totem.Runtime
{
  /// <summary>
  /// Extends <see cref="ILogger"/> with a tweaked API
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class LoggerExtensions
  {
    public static void Critical(this ILogger log, string message, params object[] args) =>
      log.LogCritical(message, args);

    public static void Critical(this ILogger log, EventId eventId, string message, params object[] args) =>
      log.LogCritical(eventId, message, args);

    public static void Critical(this ILogger log, Exception exception, string message, params object[] args) =>
      log.LogCritical(exception, message, args);

    public static void Critical(this ILogger log, EventId eventId, Exception exception, string message, params object[] args) =>
      log.LogCritical(eventId, exception, message, args);

    public static void Debug(this ILogger log, string message, params object[] args) =>
      log.LogDebug(message, args);

    public static void Debug(this ILogger log, EventId eventId, string message, params object[] args) =>
      log.LogDebug(eventId, message, args);

    public static void Debug(this ILogger log, Exception exception, string message, params object[] args) =>
      log.LogDebug(exception, message, args);

    public static void Debug(this ILogger log, EventId eventId, Exception exception, string message, params object[] args) =>
      log.LogDebug(eventId, exception, message, args);

    public static void Error(this ILogger log, string message, params object[] args) =>
      log.LogError(message, args);

    public static void Error(this ILogger log, EventId eventId, string message, params object[] args) =>
      log.LogError(eventId, message, args);

    public static void Error(this ILogger log, Exception exception, string message, params object[] args) =>
      log.LogError(exception, message, args);

    public static void Error(this ILogger log, EventId eventId, Exception exception, string message, params object[] args) =>
      log.LogError(eventId, exception, message, args);

    public static void Info(this ILogger log, string message, params object[] args) =>
      log.LogInformation(message, args);

    public static void Info(this ILogger log, EventId eventId, string message, params object[] args) =>
      log.LogInformation(eventId, message, args);

    public static void Info(this ILogger log, Exception exception, string message, params object[] args) =>
      log.LogInformation(exception, message, args);

    public static void Info(this ILogger log, EventId eventId, Exception exception, string message, params object[] args) =>
      log.LogInformation(eventId, exception, message, args);

    public static void Trace(this ILogger log, string message, params object[] args) =>
      log.LogTrace(message, args);

    public static void Trace(this ILogger log, EventId eventId, string message, params object[] args) =>
      log.LogTrace(eventId, message, args);

    public static void Trace(this ILogger log, Exception exception, string message, params object[] args) =>
      log.LogTrace(exception, message, args);

    public static void Trace(this ILogger log, EventId eventId, Exception exception, string message, params object[] args) =>
      log.LogTrace(eventId, exception, message, args);

    public static void Warning(this ILogger log, string message, params object[] args) =>
      log.LogWarning(message, args);

    public static void Warning(this ILogger log, EventId eventId, string message, params object[] args) =>
      log.LogWarning(eventId, message, args);

    public static void Warning(this ILogger log, Exception exception, string message, params object[] args) =>
      log.LogWarning(exception, message, args);

    public static void Warning(this ILogger log, EventId eventId, Exception exception, string message, params object[] args) =>
      log.LogWarning(eventId, exception, message, args);
  }
}