using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map.Diagnostics
{
  /// <summary>
  /// A collection of performance counters monitoring the runtime
  /// </summary>
  public class RuntimeMonitor : Notion
  {
    public readonly RuntimeCounterCategorySet Categories = new RuntimeCounterCategorySet();

    public void Start()
    {
      foreach(var category in Categories)
      {
        try
        {
          category.CreateLocally();
        }
        catch(Exception error)
        {
          Log.Error(error, "[runtime] Failed to create category locally: {Category}");
        }
      }
    }
  }
}