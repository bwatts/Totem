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
      WhenChanged = WhenCreated;
    }

    public DateTimeOffset WhenCreated { get; }
    public DateTimeOffset WhenChanged { get; private set; }

    internal void OnChanged() =>
      WhenChanged = Clock.Now;
  }
}