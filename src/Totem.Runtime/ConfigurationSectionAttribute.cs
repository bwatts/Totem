using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Indicates the decorated implementation of <see cref="IRuntimeArea"/> reads a configuration section
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ConfigurationSectionAttribute : Attribute
	{
		public ConfigurationSectionAttribute(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}