using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Totem.Runtime
{
  /// <summary>
  /// An object hosting a set of bindable fields, including a clock and log
  /// </summary>
  /// <remarks>
  /// .Fields is implemented as a lazy initialization to ensure deserialized durable instances
  /// have valid, instantiated sets of fields.
  /// 
  /// Other classes implementing <see cref="IBindable"/> can choose not to use the lazy
  /// instantiation if they are not durable.
  /// </remarks>
  public abstract class Notion : IBindable
  {
    [Transient]
    Fields _fields;

    [Transient, DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected Fields Fields => _fields ?? (_fields = new Fields(this));

    [Transient, DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    [Transient, DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected IClock Clock => Traits.Clock.Get(this);

    [Transient, DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected ILogger Log => Traits.Log.Get(this);

    /// <summary>
    /// Declares the Clock and Log fields common to the runtime
    /// </summary>
    public static class Traits
    {
      public static readonly Field<IClock> Clock = Field.Declare(() => Clock, new PlatformClock());
      public static readonly Field<ILogger> Log = Field.Declare(() => Log, new UninitializedLog());

      public static void SetLog(Func<IBindable, ILogger> resolve) =>
        Log.SetDefault(resolve);

      public static void ResetLog() =>
        Log.SetDefault(new UninitializedLog());

      class PlatformClock : IClock
      {
        public DateTimeOffset Now => DateTimeOffset.Now;
      }

      class UninitializedLog : ILogger
      {
        public IDisposable BeginScope<TState>(TState state) =>
          Disposal.None;

        public bool IsEnabled(LogLevel logLevel) =>
          false;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {}
      }
    }
  }
}