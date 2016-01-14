using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Totem.Reflection;
using Totem.Runtime.Map;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// Resolves contracts describing the serialization and deserialization of objects to JSON in the Totem runtime
	/// </summary>
	public class TotemContractResolver : DefaultContractResolver, ITaggable
	{
		internal TotemContractResolver()
		{
			Tags = new Tags();
		}

		Tags ITaggable.Tags { get { return Tags; } }
		private Tags Tags;
		private RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

		public bool CamelCaseProperties { get; set; } = true;

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

		protected override JsonArrayContract CreateArrayContract(Type objectType)
		{
			var contract = base.CreateArrayContract(objectType);

			if(typeof(Many<>).IsAssignableFromGeneric(objectType))
			{
				var callOf = Expression.Call(typeof(Many), "Of", new[] { contract.CollectionItemType });

				var lambda = Expression.Lambda<Func<object>>(callOf);

				contract.DefaultCreator = lambda.Compile();
			}

			return contract;
		}

		private JsonObjectContract GetCachedJsonObjectContract(ConcurrentDictionary<Type, JsonObjectContract> cache, Type objectType)
		{
			return cache.GetOrAdd(objectType, _ =>
			{
				var contract = base.CreateObjectContract(objectType);

				var mapType = Runtime.GetDurable(objectType, strict: false);

				if(mapType != null)
				{
					contract.DefaultCreator = mapType.CreateToDeserialize;
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

			return !member.IsDefined(typeof(TransientAttribute))
				&& !member.IsDefined(typeof(CompilerGeneratedAttribute))
				&& member.DeclaringType != typeof(Notion);
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