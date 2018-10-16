using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Totem.Reflection;

namespace Totem.Runtime.Json
{
  /// <summary>
  /// Resolves contracts for serializing and deserializing objects in a Totem runtime
  /// </summary>
  public class JsonFormatContractResolver : DefaultContractResolver
  {
    readonly IDurableTypeSet _durableTypes;

    public JsonFormatContractResolver(IDurableTypeSet durableTypes)
    {
      _durableTypes = durableTypes;

      NamingStrategy = new CamelCaseNamingStrategy { ProcessDictionaryKeys = false };
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

      if(_durableTypes.TryGetOrAdd(objectType, out var create))
      {
        contract.DefaultCreator = create;
      }

      return contract;
    }

    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
      if(!_durableTypes.Contains(type))
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

    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization) =>
      !IsDurableProperty(member) ? null : CreateDurableProperty(member, memberSerialization);

    static bool IsDurableProperty(MemberInfo member) =>
      !member.IsDefined(typeof(TransientAttribute))
      && !member.IsDefined(typeof(CompilerGeneratedAttribute))
      && member.DeclaringType != typeof(Notion);

    JsonProperty CreateDurableProperty(MemberInfo member, MemberSerialization memberSerialization)
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
        var info = (PropertyInfo) member;

        var canSet = info.CanWrite || info.GetSetMethod(nonPublic: true) != null;
        var canGet = info.CanRead || info.GetGetMethod(nonPublic: true) != null;

        var isWriteOnly = property
          .AttributeProvider
          .GetAttributes(typeof(WriteOnlyAttribute), inherit: true)
          .Any();

        property.Writable = !isWriteOnly && canSet;
        property.Readable = canGet;
      }

      return property;
    }
  }
}