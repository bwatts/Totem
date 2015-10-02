using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Hosting
{
	/// <summary>
	/// Describes a deployable build of the Totem runtime
	/// </summary>
	public interface IRuntimeBuild
	{
		void Deploy(IOLink location);
	}
}