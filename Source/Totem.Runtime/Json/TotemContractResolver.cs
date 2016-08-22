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
	public class TotemContractResolver : CamelCasePropertyNamesContractResolver, ITaggable
	{
		internal TotemContractResolver()
		{
			Tags = new Tags();
		}

		Tags ITaggable.Tags => Tags;
		private Tags Tags;
		private RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

		protected override JsonArrayContract CreateArrayContract(Type objectType)
		{
			var contract = base.CreateArrayContract(objectType);

			// I don't recall why this is needed - hrm

			if(typeof(Many<>).IsAssignableFromGeneric(objectType))
			{
				var callOf = Expression.Call(typeof(Many), "Of", new[] { contract.CollectionItemType });

				var lambda = Expression.Lambda<Func<object>>(callOf);

				contract.DefaultCreator = lambda.Compile();
			}

			return contract;
		}

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
      return Enumerable.Empty<MemberInfo>()
        .Concat(type.GetFields(flags))
        .Concat(type.GetProperties(flags))
        .Where(property => IsDurableProperty(property))
        .Select(member => {
          var prop = base.CreateProperty(member, memberSerialization);
          prop.Writable = true;
          prop.Readable = true;
          return prop;
        })
        .ToList();
    }

    protected override JsonObjectContract CreateObjectContract(Type objectType)
		{
			return _objectContractCache.GetOrAdd(objectType, _ =>
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

		private static readonly ConcurrentDictionary<Type, JsonObjectContract> _objectContractCache = new ConcurrentDictionary<Type, JsonObjectContract>();

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