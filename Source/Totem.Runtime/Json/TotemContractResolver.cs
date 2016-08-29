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
        .Where(IsDurableProperty)
				.Select(member => CreateProperty(member, memberSerialization))
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
			return !IsDurableProperty(member)
				? null
				: CreateDurableProperty(member, memberSerialization, base.CreateProperty(member, memberSerialization));
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

		private JsonProperty CreateDurableProperty(MemberInfo member, MemberSerialization memberSerialization, JsonProperty baseProperty)
		{
			if(member is FieldInfo)
			{
				baseProperty.Writable = true;
				baseProperty.Readable = true;
			}
			else if(member.ReflectedType != member.DeclaringType)
			{
				baseProperty = CreateProperty(member.DeclaringType.GetProperty(member.Name), memberSerialization);
			}
			else
			{
				var propertyMember = (PropertyInfo) member;

				baseProperty.Writable = propertyMember.CanWrite || propertyMember.GetSetMethod(nonPublic: true) != null;
				baseProperty.Readable = propertyMember.CanRead || propertyMember.GetGetMethod(nonPublic: true) != null;
			}

			return baseProperty;
		}
	}
}