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

    internal void Register(IEnumerable<CounterBase> declarations)
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
      DeleteIfExists();

      Create();
    }

    void DeleteIfExists()
    {
      if(PerformanceCounterCategory.Exists(Declaration.Name))
      {
        PerformanceCounterCategory.Delete(Declaration.Name);
      }
    }

    void Create()
    {
      if(Declaration.IsMultiInstance)
      {
        CreateType(PerformanceCounterCategoryType.MultiInstance);
      }
      else if(HasRuntimePrefix)
      {
        CreateType(PerformanceCounterCategoryType.MultiInstance);

        WriteDefaults();
      }
      else
      {
        CreateType(PerformanceCounterCategoryType.SingleInstance);
      }
    }

    void CreateType(PerformanceCounterCategoryType type) =>
      PerformanceCounterCategory.Create(
        Declaration.Name,
        Declaration.Description,
        type,
        new CounterCreationDataCollection(
          Counters
          .SelectMany(counter => counter.Declaration.GetCreationData(Declaration))
          .ToArray()));

    bool HasRuntimePrefix =>
      CounterBase.Traits.RuntimePrefix.ResolveDefaultTyped() != "";

    void WriteDefaults()
    {
      foreach(Counter declaration in Counters.Select(counter => counter.Declaration))
      {
        declaration.WriteDefault();
      }
    }
  }
}