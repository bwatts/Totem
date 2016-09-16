using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Totem.Runtime.Json;

namespace Totem.Runtime
{
  /// <summary>
  /// Scenarios involving the <see cref="ExpandDictionaryConverter"/> class
  /// </summary>
  public class TotemDictionaryConverterSpecs : Specs
  {
    void SerializeEmpty()
    {
      var dictionary = new Dictionary<string, int>();

      var dictionaryJson = JsonFormat.Text.Serialize(dictionary).ToString();

			Expect(dictionaryJson).Is("[]");
    }

    void Serialize()
		{
      var dictionary = new Dictionary<string, int>
      {
        ["Foo"] = 1,
        ["Bar"] = 2
      };

      var dictionaryJson = JsonFormat.Text.Serialize(dictionary).ToString();

      Expect(dictionaryJson).Is(@"[
  {
    ""key"": ""Foo"",
    ""value"": 1
  },
  {
    ""key"": ""Bar"",
    ""value"": 2
  }
]");
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