using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a context-bound set of artifacts defining the Totem runtime
	/// </summary>
	public interface IRuntimeDeployment
	{
		RuntimeContext Context { get; }

		RuntimeMap ReadMap();

		void Deploy(IFolder outputFolder);
	}
}