using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type representing a type in a timeline map
  /// </summary>
  public abstract class MapType
  {
    protected MapType(MapTypeInfo info)
    {
      DeclaredType = info.DeclaredType;
      Key = info.Key;
      State = info.State;
    }

    public readonly Type DeclaredType;
    public readonly MapTypeKey Key;
    public readonly MapTypeState State;

    public override string ToString() =>
      Key.ToString();

    public bool Is(Type type) =>
      type.IsAssignableFrom(DeclaredType);

    public bool Is<T>() =>
      Is(typeof(T));

    public bool Is(MapType type) =>
      Is(type.DeclaredType);

    public bool IsTypeOf(object instance) =>
      instance != null && instance.GetType() == DeclaredType;

    public bool CanAssign(object instance) =>
      instance != null && Is(instance.GetType());

    public bool CanAssign(MapType type) =>
      Is(type.DeclaredType);

    public Expression ConvertToDeclaredType(Expression instance) =>
      Expression.Convert(instance, DeclaredType);

    public object CreateToDeserialize() =>
      FormatterServices.GetUninitializedObject(DeclaredType);
  }
}