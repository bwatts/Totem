using System;
using Newtonsoft.Json.Serialization;
using Totem.Reflection;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Binds durable types to their area keys
  /// </summary>
  public class JsonFormatSerializationBinder : DefaultSerializationBinder
  {
    readonly IDurableTypeSet _durableTypes;

    public JsonFormatSerializationBinder(IDurableTypeSet durableTypes)
    {
      _durableTypes = durableTypes;
    }

    public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
      if(_durableTypes.TryGetKey(serializedType, out var key))
      {
        assemblyName = null;
        typeName = key;
      }
      else
      {
        base.BindToName(serializedType, out assemblyName, out typeName);
      }
    }

    public override Type BindToType(string assemblyName, string typeName) =>
      _durableTypes.TryGetByKey(typeName, out var type)
        ? type
        : TypeResolver.Resolve(typeName, assemblyName);
  }
}