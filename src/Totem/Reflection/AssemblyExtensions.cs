using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace Totem.Reflection
{
  /// <summary>
  /// Extensions to <see cref="Assembly"/>
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class AssemblyExtensions
  {
    public static string GetDirectoryName(this Assembly assembly)
    {
      var codeBase = assembly.CodeBase;

      Uri codeBaseUri;

      if(Uri.TryCreate(codeBase, UriKind.Absolute, out codeBaseUri))
      {
        codeBase = codeBaseUri.AbsolutePath;
      }

      return Path.GetDirectoryName(codeBase);
    }
  }
}