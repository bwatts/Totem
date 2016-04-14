using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// A timeline presence that responds to a pending request
	/// </summary>
	public abstract class Request : Query
	{
    [Transient] public new RequestType Type => (RequestType) base.Type;
  }
}