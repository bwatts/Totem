using System;
using Newtonsoft.Json;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Describes the format of JSON written and read in a Totem runtime
  /// </summary>
  public interface IJsonFormat
  {
    void Apply(Action<JsonSerializerSettings> operation);

    TResult Apply<TResult>(Func<JsonSerializerSettings, TResult> operation);
  }
}