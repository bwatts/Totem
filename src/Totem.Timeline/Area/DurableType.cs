using System.Runtime.Serialization;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A .NET type representing an instance that persists between usages.
  /// 
  /// In practice (so far) this means decorated with [Durable].
  /// </summary>
  public class DurableType : MapType
  {
    public DurableType(MapTypeInfo type) : base(type)
    {}

    public object CreateToDeserialize() =>
      FormatterServices.GetUninitializedObject(DeclaredType);
  }
}