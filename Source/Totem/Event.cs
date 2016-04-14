using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime;

namespace Totem
{
	/// <summary>
	/// An observable occurrence on the timeline of a distributed environment
	/// </summary>
	[Durable]
	public abstract class Event : Message
	{

	}
}