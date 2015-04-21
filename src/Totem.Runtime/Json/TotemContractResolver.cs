using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Totem.Reflection;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// Resolves contracts describing the serialization and deserialization of objects to JSON in the Totem runtime
	/// </summary>
	public class TotemContractResolver : DefaultContractResolver
	{
		public TotemContractResolver()
		{
			CamelCaseProperties = true;
		}

		public bool CamelCaseProperties { get; set; }

		protected override string ResolvePropertyName(string propertyName)
		{
			if(!CamelCaseProperties || propertyName.Length == 0 || !Char.IsUpper(propertyName[0]))
			{
				return propertyName;
			}

			return Char.ToLower(propertyName[0]) + propertyName.Substring(1);
		}

		protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			return CamelCaseProperties
				? GetCachedJsonObjectContract(_jsonObjectContractCamelCaseCache, objectType)
				: GetCachedJsonObjectContract(_jsonObjectContractNoCamelCaseCache, objectType);
		}

		private JsonObjectContract GetCachedJsonObjectContract(ConcurrentDictionary<Type, JsonObjectContract> cache, Type objectType)
		{
			return cache.GetOrAdd(objectType, _ =>
			{
				var contract = base.CreateObjectContract(objectType);

				if(objectType.IsDefined(typeof(DurableAttribute), inherit: true))
				{
					contract.DefaultCreator = () => FormatterServices.GetUninitializedObject(objectType);
				}

				return contract;
			});
		}

		private static readonly ConcurrentDictionary<Type, JsonObjectContract> _jsonObjectContractCamelCaseCache = new ConcurrentDictionary<Type, JsonObjectContract>();
		private static readonly ConcurrentDictionary<Type, JsonObjectContract> _jsonObjectContractNoCamelCaseCache = new ConcurrentDictionary<Type, JsonObjectContract>();

		//
		// Properties
		//

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			if(IsDurableProperty(member))
			{
				property.Writable = CanSet(member);
			}
			else
			{
				property = null;
			}

			return property;
		}

		private static bool IsDurableProperty(MemberInfo member)
		{
			if(member.IsDefined(typeof(DurableAttribute)))
			{
				return true;
			}

			if(member.IsDefined(typeof(TransientAttribute))
				|| member.IsDefined(typeof(CompilerGeneratedAttribute))
				|| member.DeclaringType == typeof(Notion))
			{
				return false;
			}

			var field = member as FieldInfo;

			return field == null || !(field.IsPrivate && field.IsInitOnly);
		}

		private static bool CanSet(MemberInfo member)
		{
			var canSet = false;

			if(member is FieldInfo)
			{
				canSet = true;
			}
			else
			{
				var property = member as PropertyInfo;

				if(property != null)
				{
					canSet = property.CanWrite || property.GetSetMethod(nonPublic: true) != null;
				}
			}

			return canSet;
		}
	}
}