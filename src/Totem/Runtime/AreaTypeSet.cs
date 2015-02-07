using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// The set of areas in a region
	/// </summary>
	/// <typeparam name="T">The type of runtime elements in the set</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	public sealed class AreaTypeSet : Notion, IReadOnlyCollection<AreaType>
	{
		private readonly Dictionary<RuntimeTypeKey, AreaType> _areasByKey = new Dictionary<RuntimeTypeKey, AreaType>();
		private readonly Dictionary<Type, AreaType> _areasByDeclaredType = new Dictionary<Type, AreaType>();

		public IEnumerator<AreaType> GetEnumerator()
		{
			return _areasByKey.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public int Count { get { return _areasByKey.Count; } }
		public ICollection<RuntimeTypeKey> Keys { get { return _areasByKey.Keys; } }
		public ICollection<Type> DeclaredTypes { get { return _areasByDeclaredType.Keys; } }

		public bool Contains(RuntimeTypeKey key)
		{
			return _areasByKey.ContainsKey(key);
		}

		public bool Contains(Type declaredType)
		{
			return _areasByDeclaredType.ContainsKey(declaredType);
		}

		public AreaType Get(RuntimeTypeKey key, bool strict = true)
		{
			AreaType area;

			Expect(_areasByKey.TryGetValue(key, out area) || !strict).IsTrue("Unknown runtime key", key.ToText());

			return area;
		}

		public AreaType Get(Type declaredType, bool strict = true)
		{
			AreaType area;

			Expect(_areasByDeclaredType.TryGetValue(declaredType, out area) || !strict).IsTrue("Unknown declared type", Text.Of(declaredType));

			return area;
		}

		internal void Register(AreaType area)
		{
			_areasByKey.Add(area.Key, area);

			_areasByDeclaredType.Add(area.DeclaredType, area);
		}
	}
}