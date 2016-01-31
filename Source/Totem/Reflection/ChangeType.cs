using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.Reflection
{
  /// <summary>
  /// Changes objects from their types to other types
  /// </summary>
  public static class ChangeType
  {
    public static object To(Type type, object value)
    {
      object convertedValue;

      if(value == null)
      {
        if(!type.IsAssignableNull())
        {
          throw new Exception($"Type {type} cannot be assigned null values");
        }

        convertedValue = type.GetDefaultValue();
      }
      else if(type.IsAssignableFrom(value.GetType()))
      {
        convertedValue = value;
      }
      else
      {
        ConvertibleValue convertibleValue;

        if(!TryGetConvertibleValue(value, type, out convertibleValue))
        {
          throw new Exception($"Cannot convert from {value.GetType()} to {type}");
        }

        convertedValue = convertibleValue.Convert();
      }

      return convertedValue;
    }

    public static bool TryTo(Type type, object value, out object result)
    {
      bool success;

      if(value == null)
      {
        success = type.IsAssignableNull();

        result = null;
      }
      else if(type.IsAssignableFrom(value.GetType()))
      {
        success = true;

        result = value;
      }
      else
      {
        ConvertibleValue convertibleValue;

        success = TryGetConvertibleValue(value, type, out convertibleValue);

				result = success ? convertibleValue.Convert() : type.GetDefaultValue();
      }

      return success;
    }

    public static T To<T>(object value)
    {
      return (T) To(typeof(T), value);
    }

    public static bool TryTo<T>(object value, out T result)
    {
      object untypedResult;

      var success = TryTo(typeof(T), value, out untypedResult);

      result = success ? (T) untypedResult : default(T);

      return success;
    }

    private static bool TryGetConvertibleValue(object value, Type destinationType, out ConvertibleValue convertibleValue)
    {
      // Parameter name "noCustomTypeDesc" is correct but documentation is wrong (see http://msdn.microsoft.com/en-us/library/vstudio/y7wd3zkh%28v=vs.90%29.aspx)

      var sourceTypeConverter = TypeDescriptor.GetConverter(value, noCustomTypeDesc: false);

      if(sourceTypeConverter != null && sourceTypeConverter.CanConvertTo(destinationType))
      {
        convertibleValue = new ConvertibleValueFromSourceType(value, destinationType, sourceTypeConverter);
      }
      else
      {
        var destinationTypeConverter = TypeDescriptor.GetConverter(destinationType);

        if(destinationTypeConverter.CanConvertFrom(value.GetType()))
        {
          convertibleValue = new ConvertibleValueFromDestinationType(value, destinationTypeConverter);
        }
        else
        {
          convertibleValue = null;
        }
      }

      return convertibleValue != null;
    }

    private abstract class ConvertibleValue
    {
      internal abstract object Convert();
    }

    private sealed class ConvertibleValueFromSourceType : ConvertibleValue
    {
      private readonly object _value;
      private readonly Type _destinationType;
      private readonly TypeConverter _sourceTypeConverter;

      internal ConvertibleValueFromSourceType(object value, Type destinationType, TypeConverter sourceTypeConverter)
      {
        _value = value;
        _destinationType = destinationType;
        _sourceTypeConverter = sourceTypeConverter;
      }

      internal override object Convert()
      {
        return _sourceTypeConverter.ConvertTo(_value, _destinationType);
      }
    }

    private sealed class ConvertibleValueFromDestinationType : ConvertibleValue
    {
      private readonly object _value;
      private readonly TypeConverter _destinationTypeConverter;

      internal ConvertibleValueFromDestinationType(object value, TypeConverter destinationTypeConverter)
      {
        _value = value;
        _destinationTypeConverter = destinationTypeConverter;
      }

      internal override object Convert()
      {
        return _destinationTypeConverter.ConvertFrom(_value);
      }
    }
  }
}