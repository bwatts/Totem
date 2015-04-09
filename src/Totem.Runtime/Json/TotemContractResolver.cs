using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
		public bool CamelCaseProperties { get; set; }

		protected override string ResolvePropertyName(string propertyName)
		{
			if(!CamelCaseProperties || propertyName.Length == 0 || !Char.IsUpper(propertyName[0]))
			{
				return propertyName;
			}

			var camelCasedPropertyName = Char.ToLower(propertyName[0], CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture);

			if(propertyName.Length > 1)
			{
				camelCasedPropertyName += propertyName.Substring(1);
			}

			return camelCasedPropertyName;
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

				if(IsDurable(objectType))
				{
					contract.DefaultCreator = () => FormatterServices.GetUninitializedObject(objectType);
				}

				return contract;
			});
		}

		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var property = base.CreateProperty(member, memberSerialization);

			if(member.IsDefined(typeof(TransientAttribute)))
			{
				property = null;
			}
			else
			{
				property.Writable = IsFieldOrSettableProperty(member);

				property.ShouldSerialize = value => ShouldSerialize(member);
			}

			return property;
		}

		private static bool IsFieldOrSettableProperty(MemberInfo member)
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

		//
		// Cache
		//

		private enum TypeCharacteristic { None, Durable, Anonymous }

		private static readonly ConcurrentDictionary<Type, TypeCharacteristic> _typeCache = new ConcurrentDictionary<Type, TypeCharacteristic>();
		private static readonly ConcurrentDictionary<MemberInfo, bool> _shouldSerializeCache = new ConcurrentDictionary<MemberInfo, bool>();
		private static readonly ConcurrentDictionary<Type, JsonObjectContract> _jsonObjectContractCamelCaseCache = new ConcurrentDictionary<Type, JsonObjectContract>();
		private static readonly ConcurrentDictionary<Type, JsonObjectContract> _jsonObjectContractNoCamelCaseCache = new ConcurrentDictionary<Type, JsonObjectContract>();

		internal static bool IsDurable(Type type)
		{
			return GetOrAddCharacteristic(type) == TypeCharacteristic.Durable;
		}

		internal static bool IsAnonymous(Type type)
		{
			return GetOrAddCharacteristic(type) == TypeCharacteristic.Anonymous;
		}

		internal static bool ShouldSerialize(MemberInfo member)
		{
			return _shouldSerializeCache.GetOrAdd(member, _ =>
				member.DeclaringType != typeof(Notion)
				&& (member.MemberType == MemberTypes.Property || !IsAnonymous(member.DeclaringType)));
		}

		private static TypeCharacteristic GetOrAddCharacteristic(Type type)
		{
			return _typeCache.GetOrAdd(type, _ =>
			{
				if(type.IsDefined(typeof(DurableAttribute), inherit: true))
				{
					return TypeCharacteristic.Durable;
				}
				else if(type.IsAnonymous())
				{
					return TypeCharacteristic.Anonymous;
				}
				else
				{
					return TypeCharacteristic.None;
				}
			});
		}
	}
}