using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// The state of a timeline query for the settings of a runtime area
	/// </summary>
	public abstract class SettingsView : View
	{
		protected SettingsView(Id runtimeId) : base(runtimeId)
		{}
	}
}