using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime types, indexed by key and declared type
	/// </summary>
	/// <typeparam name="T">The type of runtime types in the set</typeparam>
	public abstract class RuntimeTypeSetBase<T> : RuntimeSetBase<RuntimeTypeKey, T> where T : RuntimeType
	{
		public abstract IEnumerable<Type> DeclaredTypes { get; }
		public abstract T this[Type declaredType] { get; }
		public abstract IEnumerable<KeyValuePair<Type, T>> DeclaredTypePairs { get; }

		public abstract bool Contains(Type declaredType);

		public abstract T Get(Type declaredType, bool strict = true);
	}
}