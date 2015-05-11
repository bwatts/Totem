using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a set of typed views accessible by key
	/// </summary>
	public interface IViewDb
	{
		void Write(View view);

		void Write(IEnumerable<View> views);

		void Delete(View view);

		void Delete(IEnumerable<View> views);

		void Delete(Type viewType, ViewKey key);

		void Delete(Type viewType, IEnumerable<ViewKey> keys);

		void Delete<TView>(ViewKey key) where TView : View;

		void Delete<TView>(IEnumerable<ViewKey> keys) where TView : View;

		View Read(Type viewType, ViewKey key, bool strict = true);

		IEnumerable<View> Read(Type viewType, IEnumerable<ViewKey> keys, bool strict = true);

		TView Read<TView>(ViewKey key, bool strict = true) where TView : View;

		IEnumerable<TView> Read<TView>(IEnumerable<ViewKey> keys, bool strict = true) where TView : View;
	}
}