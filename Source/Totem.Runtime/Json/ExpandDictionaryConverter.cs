using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Totem.Reflection;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Converts dictionaries to and from JSON as key/value pairs
  /// </summary>
  public class ExpandDictionaryConverter : JsonConverter
  {
    private readonly JsonDictionaryContract _contract;

    public ExpandDictionaryConverter(JsonDictionaryContract contract)
    {
      _contract = contract;
    }

    public override bool CanConvert(Type objectType)
    {
      return objectType == _contract.UnderlyingType;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      var dictionary = (IDictionary) value;

      writer.WriteStartArray();

      foreach(var key in dictionary.Keys)
      {
        writer.WriteStartObject();

        writer.WritePropertyName("key");

        serializer.Serialize(writer, key, _contract.DictionaryKeyType);

        writer.WritePropertyName("value");

        serializer.Serialize(writer, dictionary[key], _contract.DictionaryValueType);

        writer.WriteEndObject();
      }

      writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if(reader.TokenType == JsonToken.Null)
      {
        return null;
      }

      var dictionary = (IDictionary) _contract.DefaultCreator();

      switch(reader.TokenType)
      {
        case JsonToken.StartArray:
          ReadArray(reader, serializer, dictionary);
          break;
        case JsonToken.StartObject:
          ReadObject(reader, serializer, dictionary);
          break;
        default:
          throw new JsonSerializationException($"Expected start of array or object, received: {reader.TokenType}");
      }

      return dictionary;
    }

    private void ReadArray(JsonReader reader, JsonSerializer serializer, IDictionary dictionary)
    {
      while(reader.Read() && reader.TokenType != JsonToken.EndArray)
      {
        object key = null;
        object value = null;

        while(reader.Read() && reader.TokenType != JsonToken.EndObject)
        {
          var property = reader.Value.ToString();

          if(property.Equals("key", StringComparison.OrdinalIgnoreCase))
          {
            reader.Read();

            key = serializer.Deserialize(reader, _contract.DictionaryKeyType);
          }
          else
          {
            if(property.Equals("value", StringComparison.OrdinalIgnoreCase))
            {
              reader.Read();

              value = serializer.Deserialize(reader, _contract.DictionaryValueType);
            }
          }
        }

        if(key == null)
        {
          throw new JsonSerializationException("Expected 'key' property");
        }

        dictionary.Add(key, value);
      }
    }

    private void ReadObject(JsonReader reader, JsonSerializer serializer, IDictionary dictionary)
    {
      while(reader.Read() && reader.TokenType != JsonToken.EndObject)
      {
        var key = ChangeType.To(_contract.DictionaryKeyType, reader.Value);

        reader.Read();

        var value = serializer.Deserialize(reader, _contract.DictionaryValueType);

        dictionary.Add(key, value);
      }
    }
  }
}