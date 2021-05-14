using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Totem.Queries.Encoding
{
    internal class QueryEncoderProperty
    {
        delegate object? GetValue(object target);

        readonly PropertyInfo _info;
        readonly GetValue _getValue;
        readonly QueryEncoderValue _value;

        internal QueryEncoderProperty(PropertyInfo info, Type targetType)
        {
            _info = info;
            _getValue = CompileGetValue(targetType);
            _value = new QueryEncoderValue(info.PropertyType);
        }

        internal void Write(string targetKey, object target, QueryWriter writer)
        {
            var value = _getValue(target);

            if(value == null)
            {
                return;
            }

            var propertyKey = targetKey == "" ? _info.Name : $"{targetKey}.{_info.Name}";

            _value.Write(propertyKey, value, writer);
        }

        GetValue CompileGetValue(Type targetType)
        {
            // target => ((TTarget) target).info

            var targetParameter = Expression.Parameter(typeof(object), "target");

            var lambda = Expression.Lambda<GetValue>(
                Expression.Property(Expression.Convert(targetParameter, targetType), _info),
                targetParameter);

            return lambda.Compile();
        }
    }
}