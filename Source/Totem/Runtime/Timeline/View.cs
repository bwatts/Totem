using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A timeline presence that maintains a persistent structure
  /// </summary>
  [BatchSize(ViewType.DefaultBatchSize)]
  public abstract class View : Flow
  {
    protected View()
    {
      WhenCreated = Clock.Now;
      WhenModified = WhenCreated;
    }

    public DateTime WhenCreated { get; }
    public DateTime WhenModified { get; private set; }

    public void OnModified() => WhenModified = Clock.Now;
	}
}