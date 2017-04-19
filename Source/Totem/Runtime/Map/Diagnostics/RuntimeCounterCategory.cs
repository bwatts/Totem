using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Totem.Diagnostics;

namespace Totem.Runtime.Map.Diagnostics
{
  /// <summary>
  /// A .NET type representing a set of related performance counters
  /// </summary>
  public class RuntimeCounterCategory : RuntimeType
  {
    public RuntimeCounterCategory(RuntimeTypeRef type, CounterCategory declaration) : base(type)
    {
      Declaration = declaration;
    }

    public readonly CounterCategory Declaration;
    public readonly RuntimeCounterSet Counters = new RuntimeCounterSet();

    internal void Register(IEnumerable<Counter> declarations)
    {
      foreach(var declaration in declarations)
      {
        if(declaration.CheckCardinality(Declaration))
        {
          Counters.Register(new RuntimeCounter(this, declaration));
        }
        else if(Declaration.IsSingleInstance)
        {
          Log.Warning("[runtime] Category {Category} is single-instance but counter {Counter} is multi-instance; ignoring");
        }
        else
        {
          Log.Warning("[runtime] Category {Category} is multi-instance but counter {Counter} is single-instance; ignoring");
        }
      }
    }

    internal void CreateLocally()
    {
      if(PerformanceCounterCategory.Exists(Declaration.Name))
      {
        PerformanceCounterCategory.Delete(Declaration.Name);
      }

      PerformanceCounterCategory.Create(
        Declaration.Name,
        Declaration.Description,
        PerformanceCounterCategoryType.MultiInstance,
        new CounterCreationDataCollection(GetCreationDataArray()));
    }

    CounterCreationData[] GetCreationDataArray()
    {
      return Counters.SelectMany(GetCreationData).ToArray();
    }

    IEnumerable<CounterCreationData> GetCreationData(RuntimeCounter counter)
    {
      return counter.Declaration.GetCreationData(Declaration.Name);
    }
  }
}