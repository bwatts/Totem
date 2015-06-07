using System;
using System.Collections.Generic;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing related functionality in a Totem runtime
	/// </summary>
	public sealed class AreaType : RuntimeType
	{
		internal AreaType(RuntimeTypeRef type, ViewType settingsView, Many<IOResource> deployedResources) : base(type)
		{
			HasSettings = settingsView != null;
			SettingsView = settingsView;
			Dependencies = new AreaTypeSet();
			DeployedResources = deployedResources;
		}

		public readonly bool HasSettings;
		public readonly ViewType SettingsView;
		public readonly AreaTypeSet Dependencies;
		public readonly Many<IOResource> DeployedResources;
	}
}