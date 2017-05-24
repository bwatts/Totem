using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime;

namespace Totem.Data
{
  /// <summary>
  /// The identifier of a schema in a Totem database
  /// </summary>
  public class DbSchema : Notion
  {
    public DbSchema(string name)
    {
      Unescaped = name;
      Escaped = name.StartsWith("[") && name.EndsWith("]") ? name : $"[{name}]";
    }

    public readonly string Unescaped;
    public readonly string Escaped;

    public override string ToString() => Escaped;
  }
}