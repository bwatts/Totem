using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing an area of related functionality in a Totem runtime
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

		public T ReadSettings<T>(IViewStore views, bool strict = false) where T : View
		{
			Expect(HasSettings).IsTrue("Area does not have a settings view: " + Key.ToText());

			Expect(SettingsView.Is(typeof(T))).IsTrue(Text.Of(
				"Cannot read settings type {0} for area {1}; expected {2}",
				typeof(T),
				Key,
				SettingsView.DeclaredType));

			return views.Read<T>(Key.ToString(), strict);
		}
	}
}