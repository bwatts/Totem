using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;

namespace Totem.Runtime.Reflection
{
	/// <summary>
	/// Reflects deployed types to build a map of the runtime
	/// </summary>
	internal static class RuntimeReflection
	{
		internal static RuntimeMap ReadMap(this RuntimeSection section, Assembly programAssembly)
		{
			var deployment = section.ReadDeployment(programAssembly);

			return new MapReader(deployment).ReadMap();
		}
	}
}