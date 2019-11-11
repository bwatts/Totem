using System;
using Totem.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A description of a .NET type declared in a timeline area
  /// </summary>
  public sealed class AreaTypeInfo : IEquatable<AreaTypeInfo>
  {
    AreaTypeInfo(
      Type declaredType,
      TypeName name,
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
    public readonly TypeName Name;
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

    static AreaTypeInfo TryFrom<TAssignable>(Type type, bool isEvent = false, bool isTopic = false, bool isQuery = false) =>
      typeof(TAssignable).IsAssignableFrom(type)
        ? new AreaTypeInfo(type, TypeName.From(type), isEvent, isTopic, isQuery)
        : null;
  }
}