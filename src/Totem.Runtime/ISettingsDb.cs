using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a persistent set of settings for runtime areas
	/// </summary>
	public interface ISettingsDb
	{
		SetupView ReadSetup();

		TView Read<TView>(AreaType area, bool strict = true) where TView : View;
	}
}