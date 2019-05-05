using System;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A description of a .NET type declared in a timeline area
  /// </summary>
  public sealed class AreaTypeInfo : IEquatable<AreaTypeInfo>
  {
    AreaTypeInfo(
      Type declaredType,
      AreaTypeName name,
      bool isEvent,
      bool isTopic,
      bool isQuery)
    {
      DeclaredType = declaredType;
      Name = name;
      IsEvent = isEvent;
      IsTopic = isTopic;
      IsQuery = isQuery;
    }

    public readonly Type DeclaredType;
    public readonly AreaTypeName Name;
    public readonly bool IsEvent;
    public readonly bool IsTopic;
    public readonly bool IsQuery;

    public override string ToString() =>
      Name.ToString();

    //
    // Equality
    //

    public override bool Equals(object obj) =>
      Equals(obj as AreaTypeInfo);

    public bool Equals(AreaTypeInfo other) =>
      Eq.Values(this, other).Check(x => x.DeclaredType);

    public override int GetHashCode() =>
      DeclaredType.GetHashCode();

    public static bool operator ==(AreaTypeInfo x, AreaTypeInfo y) => Eq.Op(x, y);
    public static bool operator !=(AreaTypeInfo x, AreaTypeInfo y) => Eq.OpNot(x, y);

    //
    // Factory
    //

    public static bool TryFrom(Type type, out AreaTypeInfo info)
    {
      info = TryFrom<Event>(type, isEvent: true)
        ?? TryFrom<Topic>(type, isTopic: true)
        ?? TryFrom<Query>(type, isQuery: true);

      return info != null;
    }

    static AreaTypeInfo TryFrom<TAssignable>(Type type, bool isEvent = false, bool isTopic = false, bool isQuery = false)
    {
      if(!typeof(TAssignable).IsAssignableFrom(type))
      {
        return null;
      }

      var currentType = type;
      var currentName = type.Name;

      while(currentType.IsNested)
      {
        currentType = currentType.DeclaringType;
        currentName = $"{currentType.Name}.{currentName}";
      }

      return new AreaTypeInfo(type, AreaTypeName.From(currentName), isEvent, isTopic, isQuery);
    }
  }
}