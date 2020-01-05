using System;
using System.Linq.Expressions;
using Totem.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type declared in a timeline area
  /// </summary>
  public abstract class AreaType
  {
    internal AreaType(AreaTypeInfo info)
    {
      DeclaredType = info.DeclaredType;
      Name = info.Name;
      IsEvent = info.IsEvent;
      IsTopic = info.IsTopic;
      IsQuery = info.IsQuery;
      State = new AreaTypeState(DeclaredType);
    }

    public readonly Type DeclaredType;
    public readonly TypeName Name;
    public readonly bool IsEvent;
    public readonly bool IsTopic;
    public readonly bool IsQuery;
    public readonly AreaTypeState State;

    public override string ToString() =>
      Name.ToString();

    public bool Is(Type type) =>
      type.IsAssignableFrom(DeclaredType);

    public bool Is<T>() =>
      Is(typeof(T));

    public bool Is(AreaType type) =>
      Is(type.DeclaredType);

    public bool IsTypeOf(object instance) =>
      instance != null && instance.GetType() == DeclaredType;

    public bool CanAssign(object instance) =>
      instance != null && Is(instance.GetType());

    public bool CanAssign(AreaType type) =>
      Is(type.DeclaredType);

    public Expression ConvertToDeclaredType(Expression instance) =>
      Expression.Convert(instance, DeclaredType);
  }
}