using System;
using Newtonsoft.Json;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// The format of JSON written and read in a Totem runtime
  /// </summary>
  public sealed class JsonFormat : IJsonFormat
  {
    readonly JsonSerializerSettings _settings = new JsonSerializerSettings();

    public JsonFormat(JsonSerializerSettings settings)
    {
      _settings = settings;
    }

    public void Apply(Action<JsonSerializerSettings> operation) =>
      operation(_settings);

    public TResult Apply<TResult>(Func<JsonSerializerSettings, TResult> operation) =>
      operation(_settings);
  }
}