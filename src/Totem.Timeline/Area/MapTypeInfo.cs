using System;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A description of a .NET type representing a type in a timeline map
  /// </summary>
  public class MapTypeInfo
  {
    public MapTypeInfo(AreaKey area, Type declaredType)
    {
      DeclaredType = declaredType;
      Key = MapTypeKey.From(area, ReadName());
      State = new MapTypeState(declaredType);
    }

    public readonly Type DeclaredType;
    public readonly MapTypeKey Key;
    public readonly MapTypeState State;

    public override string ToString() =>
      Key.ToString();

    string ReadName()
    {
      var type = DeclaredType;
      var name = DeclaredType.Name;

      while(type.IsNestedPublic)
      {
        type = type.DeclaringType;

        name = $"{type.Name}.{name}";
      }

      return name;
    }
  }
}