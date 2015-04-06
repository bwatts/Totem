using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a persistent set of keyed views
	/// </summary>
	public interface IViewDb
	{
		void Write(View view);

		void Write(IEnumerable<View> views);

		void Delete(View view);

		void Delete(IEnumerable<View> views);

		void Delete(Type viewType, string key);

		void Delete(Type viewType, IEnumerable<string> keys);

		void Delete<TView>(string key) where TView : View;

		void Delete<TView>(IEnumerable<string> keys) where TView : View;

		View Read(Type viewType, string key, bool strict = true);

		IEnumerable<View> Read(Type viewType, IEnumerable<string> keys, bool strict = true);

		TView Read<TView>(string key, bool strict = true) where TView : View;

		IEnumerable<TView> Read<TView>(IEnumerable<string> keys, bool strict = true) where TView : View;
	}
}