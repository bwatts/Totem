using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Describes the point of contact between the host and runtime application domains
	/// </summary>
	public interface IRuntimeBridge
	{
		void RequestRestart(string reason);

		event EventHandler Restarting;
	}
}