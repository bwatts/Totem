using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Describes a service hosting the Totem runtime
	/// </summary>
	public interface IRuntimeService
	{
    string InstanceName { get; }

		void Restart(string reason);
	}
}