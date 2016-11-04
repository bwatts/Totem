using System;
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
		public TotemContractResolver()
		{
			Tags = new Tags();

      NamingStrategy = new CamelCaseNamingStrategy();

      ExpandDictionaries = true;
		}

		Tags ITaggable.Tags => Tags;
		private Tags Tags;
		private RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

		public bool ExpandDictionaries;

    protected override JsonDictionaryContract CreateDictionaryContract(Type objectType)
    {
      var contract = base.CreateDictionaryContract(objectType);

			if(ExpandDictionaries)
			{
				contract.Converter = new ExpandDictionaryConverter(contract);
			}

      return contract;
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

    protected override JsonObjectContract CreateObjectContract(Type objectType)
    {
      var contract = base.CreateObjectContract(objectType);

      var mapType = Runtime.GetDurable(objectType, strict: false);

      if(mapType != null)
      {
        contract.DefaultCreator = mapType.CreateToDeserialize;
      }

      return contract;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      if(!IsDurableType(type))
      {
        return base.CreateProperties(type, memberSerialization);
      }

      const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

      return Enumerable.Empty<MemberInfo>()
        .Concat(type.GetFields(flags))
        .Concat(type.GetProperties(flags))
        .Where(IsDurableProperty)
        .Select(member => CreateProperty(member, memberSerialization))
        .ToList();
    }

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      return !IsDurableProperty(member) ? null : CreateDurableProperty(member, memberSerialization);
    }

    private bool IsDurableType(Type type)
    {
      return Runtime.GetDurable(type, strict: false) != null;
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

    private JsonProperty CreateDurableProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
      var property = base.CreateProperty(member, memberSerialization);

      if(member is FieldInfo)
      {
        property.Writable = true;
        property.Readable = true;
      }
      else if(member.ReflectedType != member.DeclaringType)
      {
        property = CreateProperty(member.DeclaringType.GetProperty(member.Name), memberSerialization);
      }
      else
      {
        var propertyMember = (PropertyInfo) member;

        property.Writable = propertyMember.CanWrite || propertyMember.GetSetMethod(nonPublic: true) != null;
        property.Readable = propertyMember.CanRead || propertyMember.GetGetMethod(nonPublic: true) != null;
      }

      return property;
    }
  }
}