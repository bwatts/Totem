using System.Collections;
using System.Reflection;
using System.Text.Json.Serialization;
using Totem.Core;

namespace Totem.Queries.Encoding;

internal class QueryEncoderValue
{
    readonly QueryEncoderValue? _item;
    readonly QueryEncoderProperty[]? _childProperties;

    public QueryEncoderValue(Type valueType)
    {
        var itemType = valueType
            .GetImplementedInterfaceGenericArguments(typeof(IEnumerable<>))
            .SingleOrDefault();

        if(itemType is not null)
        {
            _item = new QueryEncoderValue(itemType);
            return;
        }

        if(CanHaveChildProperties(valueType))
        {
            _childProperties = (
                from property in valueType.GetProperties()
                where property.GetCustomAttribute<JsonIgnoreAttribute>() is null
                select new QueryEncoderProperty(property, valueType)).ToArray();
        }
    }

    internal void Write(string key, object value, QueryWriter writer)
    {
        if(_item is not null)
        {
            var index = 0;

            foreach(var item in (IEnumerable) value)
            {
                _item.Write($"{key}[{index++}]", item, writer);
            }
        }
        else if(_childProperties is not null)
        {
            foreach(var childProperty in _childProperties)
            {
                childProperty.Write(key, value, writer);
            }
        }
        else
        {
            writer.Write(key, value);
        }
    }

    static bool CanHaveChildProperties(Type valueType)
    {
        if(valueType.IsGenericType && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            valueType = Nullable.GetUnderlyingType(valueType)!.GetTypeInfo();
        }

        return valueType is not null
                && valueType.IsPublic
                && !valueType.IsPointer
                && !valueType.IsPrimitive
                && !valueType.IsEnum
                && !typeof(Exception).IsAssignableFrom(valueType)
                && !typeof(MulticastDelegate).IsAssignableFrom(valueType)
                && !typeof(Task).IsAssignableFrom(valueType)
                && !typeof(Type).IsAssignableFrom(valueType)
                && !LeafTypes().Contains(valueType);

        static IEnumerable<Type> LeafTypes()
        {
            yield return typeof(Action<>);
            yield return typeof(byte[]);
            yield return typeof(char[]);
            yield return typeof(DateTime);
            yield return typeof(DBNull);
            yield return typeof(decimal);
            yield return typeof(Func<>);
            yield return typeof(Guid);
            yield return typeof(string);
            yield return typeof(ulong);
            yield return typeof(void);
        }
    }
}
