using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
  /// <summary>
  /// A timeline presence that maintains a persistent structure
  /// </summary>
  public abstract class View : Query
  {
    protected View()
    {
      WhenCreated = Clock.Now;
      WhenModified = WhenCreated;
    }

    public DateTime WhenCreated { get; }
    public DateTime WhenModified { get; private set; }

    [Transient] public new ViewType Type => (ViewType) base.Type;

    public void OnModified()
    {
      WhenModified = Clock.Now;
    }
  }
}