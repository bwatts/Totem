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
      PropertyValues = propertyValues.ToMany();
    }

    public LogEvent(string error, LogLevel level, string template, params object[] propertyValues) : base(error)
    {
      Level = level;
      Template = template;
      PropertyValues = propertyValues.ToMany();
    }

    public LogLevel Level
    {
      get { return Traits.Level.Get(this); }
      private set { Traits.Level.Set(this, value); }
    }

    public string Template
    {
      get { return Traits.Template.Get(this); }
      private set { Traits.Template.Set(this, value); }
    }

    public Many<object> PropertyValues
    {
      get { return Traits.PropertyValues.Get(this); }
      private set { Traits.PropertyValues.Set(this, value); }
    }

    public new static class Traits
    {
      public static readonly Field<LogLevel> Level = Field.Declare(() => Level);
      public static readonly Field<string> Template = Field.Declare(() => Template);
      public static readonly Field<Many<object>> PropertyValues = Field.Declare(() => PropertyValues, () => new Many<object>());
    }
  }
}