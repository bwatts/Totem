using System;
using System.Collections.Generic;
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

    public void Start(string instanceName)
    {
      InitializePrefix(instanceName);

      CreateCategoriesLocally();
    }

    void InitializePrefix(string instanceName)
    {
      Counter.Traits.InitializeRuntimePrefix(Escape(instanceName));
    }

    string Escape(string instanceName)
    {
      // https://msdn.microsoft.com/en-us/library/windows/desktop/aa373193(v=vs.85).aspx

      var prefix = EscapeCounts(EscapeCharacters(instanceName));

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