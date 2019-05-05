using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Totem.Reflection
{
  /// <summary>
  /// Resolves types in the current app domain, correcting for partial assembly names
  /// </summary>
  public static class TypeResolver
  {
    // Adapted from Newtonsoft.Json.Utilities.ReflectionUtils.SplitFullyQualifiedTypeName
    //
    // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/ReflectionUtils.cs
    //
    // and Newtonsoft.Json.Serialization.DefaultSerializationBinder.BindToType
    //
    // https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Serialization/DefaultSerializationBinder.cs

    static readonly TypeIndex _types = new TypeIndex();

    public static bool TryResolve(string name, string assembly, out Type type) =>
      _types.TryResolve(name, assembly, out type);

    public static Type Resolve(string name, string assembly)
    {
      if(!TryResolve(name, assembly, out var type))
      {
        throw new Exception($"Failed to resolve type from assembly: {name}, {assembly}");
      }

      return type;
    }

    sealed class TypeIndex
    {
      readonly ConcurrentDictionary<string, Type> _byFullName = new ConcurrentDictionary<string, Type>();

      internal bool TryResolve(string name, string assembly, out Type type)
      {
        var fullName = string.IsNullOrEmpty(assembly) ? name : $"{name}, {assembly}";

        if(!_byFullName.TryGetValue(fullName, out type) && TryLoadType(name, assembly, out type))
        {
          _byFullName.TryAdd(fullName, type);
        }

        return type != null;
      }

      bool TryLoadType(string name, string assembly, out Type type)
      {
        if(assembly == null)
        {
          type = Type.GetType(name);
        }
        else
        {
#pragma warning disable 618, 612
          var loadedAssembly = Assembly.LoadWithPartialName(assembly);
#pragma warning restore 618, 612

          if(loadedAssembly == null)
          {
            loadedAssembly = AppDomain.CurrentDomain
              .GetAssemblies()
              .Where(a => a.FullName == assembly || a.GetName().Name == assembly)
              .FirstOrDefault();
          }

          type = loadedAssembly?.GetType(name, throwOnError: false);
        }

        return type != null;
      }
    }
  }
}