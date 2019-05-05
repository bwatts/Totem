using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.Reflection
{
  /// <summary>
  /// Extensions applying to <see cref="System.Type"/>
  /// </summary>
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class TypeExtensions
  {
    public static bool IsStatic(this Type type) =>
      type.IsClass && type.IsAbstract && type.IsSealed;

    public static bool IsAssignableNull(this Type type) =>
      type.IsClass
      || type.IsInterface
      || type.IsAssignableToGeneric(typeof(Nullable<>));

    public static object GetDefaultValue(this Type type) =>
      type.IsValueType ? Activator.CreateInstance(type) : null;

    //
    // Generics
    //

    public static bool TryGetGenericDefinition(this Type type, out Type definition)
    {
      definition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;

      return definition != null;
    }

    public static bool IsAssignableFromGeneric(this Type openType, Type closedType) =>
      TryGetGenericDefinition(closedType, out var definition) && definition == openType;

    public static bool IsAssignableToGeneric(this Type closedType, Type openType) =>
      openType.IsAssignableFromGeneric(closedType);

    public static bool TryGetAssignableGenericType(this Type openType, Type closedType, out Type assignableType)
    {
      assignableType = closedType
        .GetTypeChainToObject(includeType: true)
        .FirstOrDefault(type => type.IsAssignableToGeneric(openType));

      return assignableType != null;
    }

    //
    // Inheritance chains
    //

    public static IEnumerable<Type> GetInheritanceChainTo(
      this Type type,
      Type targetType,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false)
    {
      if(includeType)
      {
        yield return type;
      }

      var currentType = type.BaseType;

      while(currentType != null && currentType != targetType)
      {
        yield return currentType;

        currentType = currentType.BaseType;
      }

      if(currentType == null)
      {
        Expect.False(requireTargetType, Text.Of("Target type {0} is not in chain of {1}", targetType, type));
      }
      else
      {
        if(includeTargetType)
        {
          yield return targetType;
        }
      }
    }

    public static IEnumerable<Type> GetInheritanceChainTo<T>(
      this Type type,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false) =>

      type.GetInheritanceChainTo(typeof(T), requireTargetType, includeType, includeTargetType);

    public static IEnumerable<Type> GetInheritanceChainToObject(
      this Type type,
      bool includeType = false,
      bool includeObject = false) =>

      type.GetInheritanceChainTo<object>(includeType: includeType, includeTargetType: includeObject);

    public static IEnumerable<Type> GetInheritanceChainFrom(
      this Type type,
      Type targetType,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false) =>

      type.GetInheritanceChainTo(targetType, requireTargetType, includeType, includeTargetType).Reverse();

    public static IEnumerable<Type> GetInheritanceChainFrom<T>(
      this Type type,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false) =>

      type.GetInheritanceChainTo<T>(requireTargetType, includeType, includeTargetType).Reverse();

    public static IEnumerable<Type> GetInheritanceChainFromObject(
      this Type type,
      bool includeType = false,
      bool includeObject = false) =>

      type.GetInheritanceChainToObject(includeType, includeObject).Reverse();

    //
    // Type chains
    //

    public static IEnumerable<Type> GetTypeChainTo(
      this Type type,
      Type targetType,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false)
    {
      foreach(var chainType in type.GetInheritanceChainTo(targetType, requireTargetType, includeType, includeTargetType))
      {
        yield return chainType;

        foreach(var chainInterface in chainType.GetInterfaces())
        {
          yield return chainInterface;
        }
      }
    }

    public static IEnumerable<Type> GetTypeChainTo<T>(
      this Type type,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false) =>

      type.GetTypeChainTo(typeof(T), requireTargetType, includeType, includeTargetType);

    public static IEnumerable<Type> GetTypeChainToObject(
      this Type type,
      bool includeType = false,
      bool includeObject = false) =>

      type.GetTypeChainTo<object>(includeType: includeType, includeTargetType: includeObject);

    public static IEnumerable<Type> GetTypeChainFrom(
      this Type type,
      Type targetType,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false) =>

      type.GetTypeChainTo(targetType, requireTargetType, includeType, includeTargetType).Reverse();

    public static IEnumerable<Type> GetTypeChainFrom<T>(
      this Type type,
      bool requireTargetType = false,
      bool includeType = false,
      bool includeTargetType = false) =>

      type.GetTypeChainTo<T>(requireTargetType, includeType, includeTargetType).Reverse();

    public static IEnumerable<Type> GetTypeChainFromObject(
      this Type type,
      bool includeType = false,
      bool includeObject = false) =>

      type.GetTypeChainToObject(includeType, includeObject).Reverse();

    //
    // Source text
    //

    public static string ToSourceText(this Type type) =>
      type.TryReadUnqualifiedType(out var unqualifiedType)
        ? unqualifiedType
        : type.ReadQualifiedType();

    static bool TryReadUnqualifiedType(this Type type, out Text unqualifiedType)
    {
      unqualifiedType = "";

      if(type.IsEnum)
      {
        return false;
      }

      if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        unqualifiedType = type.GetGenericArguments().Single().ToSourceText() + "?";

        return true;
      }

      var primitiveType = type.ReadPrimitiveType();

      var isPrimitiveType = primitiveType != "";

      if(isPrimitiveType)
      {
        unqualifiedType = primitiveType;
      }

      return isPrimitiveType;
    }

    static string ReadPrimitiveType(this Type type)
    {
      switch(Type.GetTypeCode(type))
      {
        case TypeCode.Boolean:
          return "bool";
        case TypeCode.Char:
          return "char";
        case TypeCode.Byte:
          return "byte";
        case TypeCode.Int16:
          return "short";
        case TypeCode.Int32:
          return "int";
        case TypeCode.Int64:
          return "long";
        case TypeCode.Single:
          return "float";
        case TypeCode.Double:
          return "double";
        case TypeCode.Decimal:
          return "decimal";
        case TypeCode.DateTime:
          return "DateTime";
        case TypeCode.String:
          return "string";
        default:
          return "";
      }
    }

    static Text ReadQualifiedType(this Type type)
    {
      var name = type.Name;

      if(type.IsNested)
      {
        name = $"{type.DeclaringType.ToSourceText()}.{name}";
      }

      var backtickIndex = name.IndexOf('`');

      if(backtickIndex == -1)
      {
        return name;
      }
      else
      {
        name = name.Substring(0, backtickIndex);

        var typeArguments = type
          .GetGenericArguments()
          .ToTextSeparatedBy(", ", arg => arg.ToSourceText());

        return $"{name}<{typeArguments}>";
      }
    }
  }
}