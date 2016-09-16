using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Totem.Runtime.Json;
using Totem.Runtime.Timeline;

namespace Totem.Runtime
{
  /// <summary>
  /// Scenarios involving the <see cref="TotemDictionaryConverter"/> class
  /// </summary>
  public class TotemDictionaryConverterSpecs : Specs
  {
    void SerializeEmpty()
    {
      var pairs = new List<KeyValuePair<string, int>>();
      var dictionary = new Dictionary<string, int>();

      var pairsJson = JsonFormat.Text.Serialize(pairs).ToString();
      var dictionaryJson = JsonFormat.Text.Serialize(dictionary).ToString();

      Expect(pairsJson).Is(dictionaryJson);
    }

    void Serialize()
		{
      var pairs = new List<KeyValuePair<string, int>>
      {
        new KeyValuePair<string, int>("Foo", 1),
        new KeyValuePair<string, int>("Bar", 2)
      };

      var dictionary = new Dictionary<string, int>
      {
        ["Foo"] = 1,
        ["Bar"] = 2
      };

      var pairsJson = JsonFormat.Text.Serialize(pairs).ToString();
      var dictionaryJson = JsonFormat.Text.Serialize(dictionary).ToString();

      Expect(pairsJson).Is(dictionaryJson);
		}

    //
    // Array
    //

    void DeserializeFromEmptyArray()
    {
      var dictionary = JsonFormat.Text.Deserialize<Dictionary<string, int>>("[]");

      Expect(dictionary.Count).Is(0);
    }

    void DeserializeFromArray()
    {
      var json = @"[
{ ""key"": ""Foo"", ""value"": 1 },
{ ""key"": ""Bar"", ""value"": 2 },
]";

      var dictionary = JsonFormat.Text.Deserialize<Dictionary<string, int>>(json);

      Expect(dictionary.Count).Is(2);

      Expect(dictionary.ContainsKey("Foo"));
      Expect(dictionary["Foo"]).Is(1);

      Expect(dictionary.ContainsKey("Bar"));
      Expect(dictionary["Bar"]).Is(2);
    }

    void DeserializeFromArrayMissingKey()
    {
      var json = @"[
{ ""key"": ""Foo"", ""value"": 1 },
{ ""value"": 2 }
]";

      ExpectThrows<JsonSerializationException>(() => JsonFormat.Text.Deserialize(json, typeof(Dictionary<string, int>)));
    }

    void DeserializeFromArrayMissingValue()
    {
      var json = @"[
{ ""key"": ""Foo"", ""value"": ""1"" },
{ ""key"": ""Bar"" }
]";

      var dictionary = JsonFormat.Text.Deserialize<Dictionary<string, string>>(json);

      Expect(dictionary.Count).Is(2);

      Expect(dictionary.ContainsKey("Foo"));
      Expect(dictionary["Foo"]).Is("1");

      Expect(dictionary.ContainsKey("Bar"));
      Expect(dictionary["Bar"]).Is(null);
    }

    //
    // Object
    //

    void DeserializeFromEmptyObject()
    {
      var dictionary = JsonFormat.Text.Deserialize<Dictionary<string, int>>("{}");

      Expect(dictionary.Count).Is(0);
    }

    void DeserializeFromObject()
    {
      var json = @"{
  ""foo"": 1,
  ""bar"": 2
}";

      var dictionary = JsonFormat.Text.Deserialize<Dictionary<string, int>>(json);

      Expect(dictionary.Count).Is(2);

      Expect(dictionary.ContainsKey("foo"));
      Expect(dictionary["foo"]).Is(1);

      Expect(dictionary.ContainsKey("bar"));
      Expect(dictionary["bar"]).Is(2);
    }

    void DeserializeFromObjectWithTypedKey()
    {
      var json = @"{
  ""1"": ""foo"",
  ""2"": ""bar""
}";

      var dictionary = JsonFormat.Text.Deserialize<Dictionary<int, string>>(json);

      Expect(dictionary.Count).Is(2);

      Expect(dictionary.ContainsKey(1));
      Expect(dictionary[1]).Is("foo");

      Expect(dictionary.ContainsKey(2));
      Expect(dictionary[2]).Is("bar");
    }
  }
}