using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a persistent set of settings for areas of a runtime
	/// </summary>
	public interface ISettingsDb
	{
		TView ReadViewOrNull<TView>() where TView : View;
	}
}