using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of named values written and read by a flow
	/// </summary>
	/// <typeparam name="T">The type of message-bound methods in the set</typeparam>
	public class RuntimeState : Notion, IReadOnlyDictionary<string, RuntimeStatePart>
	{
		private readonly Dictionary<string, RuntimeStatePart> _partsByName;

		public RuntimeState(IEnumerable<RuntimeStatePart> parts)
		{
			_partsByName = parts.ToDictionary(part => part.Name);
		}

		public RuntimeState(Type type)
		{
			var parts =
				from member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				where member.MemberType.HasFlag(MemberTypes.Field | MemberTypes.Property)
				where !member.IsDefined(typeof(CompilerGeneratedAttribute))
				select new RuntimeStatePart(member);

			_partsByName = parts.ToDictionary(part => part.Name);
		}

		public int Count { get { return _partsByName.Count; } }
		public bool Any { get { return _partsByName.Count > 0; } }
		public IEnumerable<string> Names { get { return _partsByName.Keys; } }
		public IEnumerable<RuntimeStatePart> Parts { get { return _partsByName.Values; } }
		public RuntimeStatePart this[string name] { get { return _partsByName[name]; } }

		public IEnumerator<RuntimeStatePart> GetEnumerator()
		{
			return _partsByName.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool ContainsName(string name)
		{
			return _partsByName.ContainsKey(name);
		}

		public RuntimeStatePart Get(string name, bool strict = true)
		{
			if(!ContainsName(name))
			{
				Expect(strict).IsFalse("Unknown part name: " + Text.Of(name));

				return null;
			}

			return _partsByName[name];
		}

		//
		// IReadOnlyDictionary
		//

		IEnumerable<string> IReadOnlyDictionary<string, RuntimeStatePart>.Keys { get { return _partsByName.Keys; } }
		IEnumerable<RuntimeStatePart> IReadOnlyDictionary<string, RuntimeStatePart>.Values { get { return _partsByName.Values; } }

		bool IReadOnlyDictionary<string, RuntimeStatePart>.ContainsKey(string key)
		{
			return _partsByName.ContainsKey(key);
		}

		bool IReadOnlyDictionary<string, RuntimeStatePart>.TryGetValue(string key, out RuntimeStatePart value)
		{
			return _partsByName.TryGetValue(key, out value);
		}

		IEnumerator<KeyValuePair<string, RuntimeStatePart>> IEnumerable<KeyValuePair<string, RuntimeStatePart>>.GetEnumerator()
		{
			return _partsByName.GetEnumerator();
		}
	}
}