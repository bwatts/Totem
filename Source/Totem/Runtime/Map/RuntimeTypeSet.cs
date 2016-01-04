using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of runtime types with no further differentiation
	/// </summary>
	/// <typeparam name="T">The type of runtime types in the set</typeparam>
	public class RuntimeTypeSet<T> : RuntimeTypeSetBase<T> where T : RuntimeType
	{
		private readonly Dictionary<RuntimeTypeKey, T> _typesByKey = new Dictionary<RuntimeTypeKey, T>();
		private readonly Dictionary<Type, T> _typesByDeclaredType = new Dictionary<Type, T>();

		public override int Count => _typesByKey.Count;
		public override T this[RuntimeTypeKey key] => _typesByKey[key];
		public override IEnumerable<RuntimeTypeKey> Keys => _typesByKey.Keys;
		public override IEnumerable<T> Values => _typesByKey.Values;
		public override IEnumerable<KeyValuePair<RuntimeTypeKey, T>> Pairs => _typesByKey;

		public override bool Contains(RuntimeTypeKey key)
		{
			return _typesByKey.ContainsKey(key);
		}

		public override T Get(RuntimeTypeKey key, bool strict = true)
		{
			T type;

			Expect(_typesByKey.TryGetValue(key, out type) && strict).IsFalse("Unknown domain key: " + Text.Of(key));

			return type;
		}

		//
		// Declared types
		//

		public override IEnumerable<Type> DeclaredTypes => _typesByDeclaredType.Keys;
		public override IEnumerable<KeyValuePair<Type, T>> DeclaredTypePairs => _typesByDeclaredType;
		public override T this[Type declaredType] => _typesByDeclaredType[declaredType];

		public override bool Contains(Type declaredType)
		{
			return _typesByDeclaredType.ContainsKey(declaredType);
		}

		public override T Get(Type declaredType, bool strict = true)
		{
			T type;

			if(!_typesByDeclaredType.TryGetValue(declaredType, out type) && strict)
			{
				throw new KeyNotFoundException("Unknown declared type: " + Text.Of(declaredType));
			}

			return type;
		}

		protected internal void RegisterIfNotAlready(T type)
		{
			if(!Contains(type.Key))
			{
				Register(type);
			}
		}

		protected internal void Register(T type)
		{
			RegisterByKey(type);

			RegisterByOtherKeys(type);
		}

		private void RegisterByKey(T type)
		{
			if(Contains(type.Key))
			{
				throw new Exception(Text.None
					.WriteLine("The key {0} is associated with multiple types", type.Key)
					.WriteLine()
					.WriteLine(_typesByKey[type.Key])
					.Write(type));
			}

			_typesByKey.Add(type.Key, type);

			if(Contains(type.DeclaredType))
			{
				throw new Exception(Text.None
					.WriteLine("The declared type {0} is associated with multiple types", type.DeclaredType)
					.WriteLine()
					.WriteLine(_typesByDeclaredType[type.DeclaredType])
					.Write(type));
			}

			_typesByDeclaredType.Add(type.DeclaredType, type);
		}

		internal virtual void RegisterByOtherKeys(T type)
		{}
	}
}