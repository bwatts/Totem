using System;
using System.ComponentModel;

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
        Expect.True(type.IsAssignableNull(), $"Type {type} cannot be assigned null values");

        convertedValue = type.GetDefaultValue();
      }
      else if(type.IsAssignableFrom(value.GetType()))
      {
        convertedValue = value;
      }
      else
      {
        Expect.True(TryGetConvertibleValue(value, type, out var convertibleValue), $"Cannot convert from {value.GetType()} to {type}");

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
        success = TryGetConvertibleValue(value, type, out var convertibleValue);

        result = success ? convertibleValue.Convert() : type.GetDefaultValue();
      }

      return success;
    }

    public static T To<T>(object value) =>
      (T) To(typeof(T), value);

    public static bool TryTo<T>(object value, out T result)
    {
      var success = TryTo(typeof(T), value, out var untypedResult);

      result = success ? (T) untypedResult : default(T);

      return success;
    }

    static bool TryGetConvertibleValue(object value, Type destinationType, out ConvertibleValue convertibleValue)
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

    abstract class ConvertibleValue
    {
      internal abstract object Convert();
    }

    class ConvertibleValueFromSourceType : ConvertibleValue
    {
      readonly object _value;
      readonly Type _destinationType;
      readonly TypeConverter _sourceTypeConverter;

      internal ConvertibleValueFromSourceType(object value, Type destinationType, TypeConverter sourceTypeConverter)
      {
        _value = value;
        _destinationType = destinationType;
        _sourceTypeConverter = sourceTypeConverter;
      }

      internal override object Convert() =>
        _sourceTypeConverter.ConvertTo(_value, _destinationType);
    }

    class ConvertibleValueFromDestinationType : ConvertibleValue
    {
      readonly object _value;
      readonly TypeConverter _destinationTypeConverter;

      internal ConvertibleValueFromDestinationType(object value, TypeConverter destinationTypeConverter)
      {
        _value = value;
        _destinationTypeConverter = destinationTypeConverter;
      }

      internal override object Convert() =>
        _destinationTypeConverter.ConvertFrom(_value);
    }
  }
}