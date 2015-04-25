using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Configuration;
using Totem.Runtime.Map;

namespace Totem.Runtime.Reflection
{
	/// <summary>
	/// Reflects deployed types to build a map of the runtime
	/// </summary>
	internal static class RuntimeReflection
	{
		internal static RuntimeMap ReadMap(this RuntimeSection section)
		{
			return new MapReader(section.ReadDeployment()).ReadMap();
		}
	}
}