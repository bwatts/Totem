using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Diagnostics
{
  /// <summary>
  /// Base implementation of a monitored aspect of runtime performance
  /// </summary>
  /// <remarks>
  /// All counters have instances per runtime. If a particular counter is multi-instance,
  /// the runtime serves as its parent. See:
  /// 
  /// https://msdn.microsoft.com/en-us/library/windows/desktop/aa373193(v=vs.85).aspx
  /// </remarks>
  public abstract class CounterBase : Binding
  {
    protected CounterBase(string name, string description)
    {
      Name = name;
      Description = description;
    }

    public readonly string Name;
    public readonly string Description;

    public override string ToString() => Name;

    internal abstract bool CheckCardinality(CounterCategory category);

    internal abstract IEnumerable<CounterCreationData> GetCreationData(CounterCategory category);

    protected CounterCreationData NewData(PerformanceCounterType type)
    {
      return new CounterCreationData(Name, Description, type);
    }

    protected CounterCreationData NewBaseData(PerformanceCounterType type, string suffix = " [base]")
    {
      return new CounterCreationData(Name + suffix, Description, type);
    }

    protected string RuntimePrefix
    {
      get { return Traits.RuntimePrefix.Get(this); }
      set { Traits.RuntimePrefix.Set(this, value); }
    }

    internal static class Traits
    {
      internal static readonly Field<string> RuntimePrefix = Field.Declare(() => RuntimePrefix, "");

      public static void InitializeRuntimePrefix(string value)
      {
        Expect(RuntimePrefix.ResolveDefault()).Is("", "The .RuntimePrefix trait is already initialized");

        RuntimePrefix.SetDefault(value);
      }
    }
  }
}