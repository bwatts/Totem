using System;
using Totem.Timeline.Area;

namespace Totem.Timeline
{
  /// <summary>
  /// A timeline presence maintaining a persistent data structure
  /// </summary>
  [QueryBatchSize(QueryType.DefaultBatchSize)]
  public abstract class Query : Flow
  {
    protected Query()
    {
      WhenCreated = Clock.Now;
      WhenUpdated = WhenCreated;
    }

    public DateTimeOffset WhenCreated { get; }
    public DateTimeOffset WhenUpdated { get; private set; }

    internal void OnUpdated() =>
      WhenUpdated = Clock.Now;
  }
}