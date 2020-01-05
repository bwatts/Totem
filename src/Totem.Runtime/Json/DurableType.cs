using System;
using System.Reflection;
using System.Runtime.Serialization;
using Totem.Reflection;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// A type known to Totem JSON serialization
  /// </summary>
  public class DurableType
  {
    public DurableType(DurableTypeKey key, Type declaredType)
    {
      Key = key;
      DeclaredType = declaredType;
    }

    public readonly DurableTypeKey Key;
    public readonly Type DeclaredType;

    public override string ToString() =>
      Key.ToString();

    public object Create() =>
      FormatterServices.GetUninitializedObject(DeclaredType);

    //
    // Factory
    //

    public static bool IsDurable(Type declaredType)
    {
      var currentType = declaredType;

      do
      {
        if(currentType.IsDefined(typeof(DurableAttribute), inherit: true))
        {
          return true;
        }

        currentType = currentType.DeclaringType;
      }
      while(currentType != null);

      return false;
    }

    public static bool TryFrom(Type declaredType, out DurableType type)
    {
      type = IsDurable(declaredType) && TryGetKey(declaredType, out var key)
        ? new DurableType(key, declaredType)
        : null;

      return type != null;
    }

    public static bool TryFrom(DurablePrefix prefix, Type declaredType, out DurableType type)
    {
      type = IsDurable(declaredType)
        ? CreateType(prefix, declaredType)
        : null;

      return type != null;
    }

    public static bool TryFrom(string prefix, Type declaredType, out DurableType type)
    {
      type = DurablePrefix.TryFrom(prefix, out var parsedPrefix)
        ? CreateType(parsedPrefix, declaredType)
        : null;

      return type != null;
    }

    public static DurableType From(DurablePrefix prefix, Type declaredType)
    {
      if(!TryFrom(prefix, declaredType, out var type))
      {
        throw new ArgumentException($"Type is not durable: {declaredType}", nameof(declaredType));
      }

      return type;
    }

    public static DurableType From(string prefix, Type declaredType) =>
      From(DurablePrefix.From(prefix), declaredType);

    public static DurableType From(Type declaredType)
    {
      if(!TryFrom(declaredType, out var type))
      {
        throw new ArgumentException($"Type is not durable: {declaredType}", nameof(declaredType));
      }

      return type;
    }

    //
    // Details
    //

    static bool TryGetKey(Type declaredType, out DurableTypeKey key)
    {
      var value =
        declaredType.GetCustomAttribute<DurablePrefixAttribute>()?.Prefix
        ?? declaredType.Assembly.GetCustomAttribute<DurablePrefixAttribute>()?.Prefix
        ?? "";

      key = DurablePrefix.TryFrom(value, out var prefix)
        ? DurableTypeKey.From(prefix, TypeName.From(declaredType))
        : null;

      return key != null;
    }

    static DurableType CreateType(DurablePrefix prefix, Type declaredType) =>
      new DurableType(
        DurableTypeKey.From(prefix, TypeName.From(declaredType)),
        declaredType);
  }
}