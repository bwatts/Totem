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

      CreateCategoriesLocally();
    }

    void InitializePrefix(string instance)
    {
      CounterBase.Traits.InitializeRuntimePrefix(Escape(instance));
    }

    string Escape(string instance)
    {
      // https://msdn.microsoft.com/en-us/library/windows/desktop/aa373193(v=vs.85).aspx

      var prefix = EscapeCounts(EscapeCharacters(instance));

      return prefix == "" ? "<default>" : prefix;
    }

    string EscapeCharacters(string prefix) =>
      prefix
      .Replace(@"\", "-")
      .Replace("(", "[")
      .Replace(")", "]")
      .Replace("/", "-")
      .Replace("*", "-");

    string EscapeCounts(string prefix) =>
      Regex.Replace(prefix, "#[0-9][^0-9]", match => "-" + match.Groups[0].Value);

    void CreateCategoriesLocally()
    {
      foreach(var category in Categories)
      {
        try
        {
          CreateLocally(category);
        }
        catch(Exception error)
        {
          Log.Error(error, "[runtime] Failed to create category locally: {Category}", category);
        }
      }
    }


    void CreateLocally(RuntimeCounterCategory category)
    {
      TryResetCounters();
      try
      {
        category.CreateLocally();
      }
      catch(InvalidOperationException error)
      {
        if(!IsCorrupted(error))
        {
          throw;
        }

        TryResetCounters();

        category.CreateLocally();
      }
    }

    static bool IsCorrupted(InvalidOperationException error) =>
      error.Message.StartsWith(
        "Cannot load Counter Name data because an invalid index",
        StringComparison.OrdinalIgnoreCase);

    void TryResetCounters()
    {
      Log.Info("[runtime] Detected counter corruption. Resetting...");

      try
      {
        using(var process = new Process
        {
          StartInfo = new ProcessStartInfo
          {
            FileName = "lodctr",
            Arguments = "/r",
            UseShellExecute = false
          }
        })
        {
          process.Start();
          process.WaitForExit();

          Console.WriteLine();
        }
      }
      catch(Exception resetError)
      {
        Log.Error(resetError, "[runtime] Failed to reset counters");
      }
    }
  }
}