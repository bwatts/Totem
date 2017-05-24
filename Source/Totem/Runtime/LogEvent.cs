using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
  /// <summary>
  /// A signal to Serilog on the timeline of a distributed environment
  /// </summary>
  public class LogEvent : Event
  {
    public LogEvent(LogLevel level, string template, params object[] propertyValues)
    {
      Level = level;
      Template = template;
      PropertyValues = propertyValues;
    }

    public LogEvent(Exception error, LogLevel level, string template, params object[] propertyValues)
    {
      Level = level;
      Template = template;
      PropertyValues = propertyValues;
      Error = error;
    }

    public readonly LogLevel Level;
    public readonly Text Template;
    public readonly object[] PropertyValues;
    public readonly Exception Error;
  }
}