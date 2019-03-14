using System;
using System.Reflection;
using Totem.Reflection;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A named value (field or property) in the state of a timeline type
  /// </summary>
  public class AreaTypeStatePart
  {
    internal AreaTypeStatePart(MemberInfo member)
    {
      Member = member;
      Name = member.Name;

      AsField = member as FieldInfo;
      AsProperty = member as PropertyInfo;

      IsField = AsField != null;
      IsProperty = AsProperty != null;

      if(IsField)
      {
        ValueType = AsField.FieldType;

        var canWrite = !AsField.IsInitOnly;

        GetIsPublic = AsField.IsPublic;
        SetIsPublic = GetIsPublic && canWrite;

        GetIsInternal = AsField.IsFamily;
        SetIsInternal = GetIsInternal && canWrite;

        GetIsProtected = AsField.IsFamily;
        SetIsProtected = GetIsProtected && canWrite;

        GetIsPrivate = AsField.IsPrivate;
        SetIsPrivate = GetIsPrivate && canWrite;
      }
      else
      {
        ValueType = AsProperty.PropertyType;
        
        var getter = AsProperty.GetGetMethod(nonPublic: true);
        var setter = AsProperty.GetSetMethod(nonPublic: true);

        GetIsPublic = getter != null && getter.IsPublic;
        SetIsPublic = setter != null && setter.IsPublic;

        GetIsInternal = getter != null && getter.IsAssembly;
        SetIsInternal = setter != null && setter.IsAssembly;

        GetIsProtected = getter != null && getter.IsFamily;
        SetIsProtected = setter != null && setter.IsFamily;

        GetIsPrivate = getter != null && getter.IsPrivate;
        SetIsPrivate = setter != null && setter.IsPrivate;
      }
    }

    public readonly MemberInfo Member;
    public readonly string Name;
    public readonly Type ValueType;
    public readonly bool IsField;
    public readonly bool IsProperty;
    public readonly FieldInfo AsField;
    public readonly PropertyInfo AsProperty;
    public readonly bool GetIsPublic;
    public readonly bool SetIsPublic;
    public readonly bool GetIsInternal;
    public readonly bool SetIsInternal;
    public readonly bool GetIsProtected;
    public readonly bool SetIsProtected;
    public readonly bool GetIsPrivate;
    public readonly bool SetIsPrivate;

    public override string ToString() => $"{ValueType.ToSourceText()} {Name}";
  }
}