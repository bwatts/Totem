using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Totem.Diagnostics;

namespace Totem.Runtime.Map.Diagnostics
{
  /// <summary>
  /// A collection of performance counters monitoring the runtime
  /// </summary>
  public class RuntimeMonitor : Notion
  {
    public readonly RuntimeCounterCategorySet Categories = new RuntimeCounterCategorySet();

    public void Start(string instance)
    {
      InitializePrefix(instance);

      try
      {
        CreateCounters();
      }
      catch(Exception error)
      {
        Log.Error(error, "[runtime] Failed to start runtime monitor");
      }
    }

    //
    // Prefix
    //

    void InitializePrefix(string instance) =>
      CounterBase.Traits.InitializeRuntimePrefix(EscapePrefix(instance));

    // https://msdn.microsoft.com/en-us/library/windows/desktop/aa373193(v=vs.85).aspx

    string EscapePrefix(string instance) =>
      EscapeCounts(EscapeCharacters(instance));

    string EscapeCharacters(string prefix) =>
      prefix
      .Replace(@"\", "-")
      .Replace("(", "[")
      .Replace(")", "]")
      .Replace("/", "-")
      .Replace("*", "-");

    string EscapeCounts(string prefix) =>
      Regex.Replace(prefix, "#[0-9][^0-9]", match => "-" + match.Groups[0].Value);

    //
    // Counters
    //

    void CreateCounters()
    {
      try
      {
        CreateCategories();
      }
      catch(Exception error)
      {
        if(TryResetCounters(error))
        {
          CreateCategories();
        }
        else
        {
          throw;
        }
      }
    }

    void CreateCategories()
    {
      foreach(var category in Categories)
      {
        try
        {
          category.CreateLocally();
        }
        catch(Exception error)
        {
          Log.Error(error, "[runtime] Failed to create counter category locally: {Category}", category);
        }
      }
    }

    bool TryResetCounters(Exception error)
    {
      if(IsCorrupted(error))
      {
        Log.Info("[runtime] Detected counter corruption. Resetting...");

        ResetCounters();

        return true;
      }

      return false;
    }

    static bool IsCorrupted(Exception error) =>
      error.Message.StartsWith(
        "Cannot load Counter Name data because an invalid index",
        StringComparison.OrdinalIgnoreCase);

    void ResetCounters()
    {
      try
      {
        using(var process = new Process
        {
          StartInfo = new ProcessStartInfo
          {
            FileName = "lodctr",
            Arguments = "/r",
            UseShellExecute = true
          }
        })
        {
          process.Start();
          process.WaitForExit();
        }
      }
      catch(Exception resetError)
      {
        Log.Error(resetError, "[runtime] Failed to reset counters");
      }
    }
  }
}