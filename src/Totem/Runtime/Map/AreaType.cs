using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing related functionality in a Totem runtime
	/// </summary>
	public sealed class AreaType : RuntimeType
	{
		internal AreaType(RuntimeTypeRef type, ViewType settingsView) : base(type)
		{
			HasSettings = settingsView != null;
			SettingsView = settingsView;
			Dependencies = new AreaTypeSet();
		}

		public readonly bool HasSettings;
		public readonly ViewType SettingsView;
		public readonly AreaTypeSet Dependencies;
	}
}