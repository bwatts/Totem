using System;
using EventStore.ClientAPI;
using Totem.Runtime;

namespace Totem.Timeline.EventStore.Hosting
{
  /// <summary>
  /// Writes log messages from the EventStore client to the runtime log
  /// </summary>
  public class EventStoreLogAdapter : Notion, ILogger
  {
    public void Debug(string format, params object[] args) =>
      Log.Debug(format, args);

    public void Debug(Exception ex, string format, params object[] args) =>
      Log.Debug(ex, format, args);

    public void Error(string format, params object[] args) =>
      Log.Error(format, args);

    public void Error(Exception ex, string format, params object[] args) =>
      Log.Error(ex, format, args);

    public void Info(string format, params object[] args) =>
      Log.Info(format, args);

    public void Info(Exception ex, string format, params object[] args) =>
      Log.Info(ex, format, args);
  }
}