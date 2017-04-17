using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Totem.Runtime;
using Totem.Runtime.Map;

namespace Totem
{
  /// <summary>
  /// A bindable object with access to a clock, a log, and a map of the runtime
  /// </summary>
  public abstract class Notion : Binding
  {
    [DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected IClock Clock => Traits.Clock.Get(this);

    [DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected ILog Log => Traits.Log.Get(this);

    [DebuggerHidden, DebuggerNonUserCode, DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected RuntimeMap Runtime => Traits.Runtime.Get(this);

    public static class Traits
    {
      public static readonly Field<IClock> Clock = Field.Declare(() => Clock, new PlatformClock());
      public static readonly Field<RuntimeMap> Runtime = Field.Declare(() => Runtime);
      public static readonly Field<ILog> Log = Field.Declare(() => Log, new UninitializedLog());

      class PlatformClock : IClock
      {
        public DateTime Now => DateTime.UtcNow;
      }

      public static void InitializeRuntime(RuntimeMap runtime)
      {
        Expect(Runtime.ResolveDefault()).IsNull("The .Runtime trait is already initialized");

        Runtime.SetDefault(runtime);
      }

      public static RuntimeMap ResolveRuntime()
      {
        return Runtime.ResolveDefaultTyped();
      }

      public static void InitializeLog(ILog effectiveLog)
      {
        var uninitializedLog = Log.ResolveDefault() as UninitializedLog;

        Expect(uninitializedLog).IsNotNull("The .Log trait is already initialized");

        Log.SetDefault(uninitializedLog);

        uninitializedLog.ReplayInto(uninitializedLog);
      }

      class UninitializedLog : ILog
      {
        readonly BlockingCollection<LogEvent> _events = new BlockingCollection<LogEvent>();
        ILog _effectiveLog;

        public LogLevel Level => LogLevel.Inherit;

        public void Write(LogEvent e)
        {
          if(_effectiveLog == null)
          {
            _events.Add(e);
          }
          else
          {
            _effectiveLog.Write(e);
          }
        }

        internal void ReplayInto(ILog effectiveLog)
        {
          _effectiveLog = effectiveLog;

          _events.CompleteAdding();

          foreach(var e in _events.GetConsumingEnumerable())
          {
            _effectiveLog.Write(e);
          }
        }
      }
    }
  }
}