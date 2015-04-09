using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// Resolves type names by working around a minor bug in JSON.NET
	/// </summary>
	internal sealed class TotemSerializationBinder : DefaultSerializationBinder
	{
		public override Type BindToType(string assemblyName, string typeName)
		{
			return TotemJson.ResolveType(typeName, assemblyName);
		}
	}
}