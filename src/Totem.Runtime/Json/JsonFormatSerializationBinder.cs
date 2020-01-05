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
    const string _prefix = "durable:";

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
        typeName = $"{_prefix}{key}";
      }
      else
      {
        base.BindToName(serializedType, out assemblyName, out typeName);
      }
    }

    public override Type BindToType(string assemblyName, string typeName)
    {
      if(typeName.StartsWith(_prefix))
      {
        var key = typeName.Substring(_prefix.Length);

        if(DurableTypeKey.TryFrom(key, out var parsedKey) && _durableTypes.TryGetByKey(parsedKey, out var type))
        {
          return type;
        }
      }

      return TypeResolver.Resolve(typeName, assemblyName);
    }
  }
}