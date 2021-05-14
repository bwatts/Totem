using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Totem
{
    /// <summary>
    /// A <see cref="string"/> that is not <see langword="null"/> or whitespace
    /// </summary>
    [JsonConverter(typeof(TextJsonConverter))]
    [TypeConverter(typeof(TextTypeConverter))]
    public sealed class Text : IEquatable<Text>, IComparable<Text>, IEquatable<string?>, IComparable<string?>
    {
        readonly string _value;

        Text(string value) => _value = value;

        public override string ToString() => _value;
        public override int GetHashCode() => _value.GetHashCode();
        public override bool Equals(object? obj) =>
          obj switch
          {
              Text other => _value == other._value,
              string other => _value == other,
              _ => false
          };

        public bool Equals(Text? other) => _value == other?._value;
        public bool Equals(string? other) => _value == other;

        public int CompareTo(Text? other) => other == null ? 1 : _value.CompareTo(other._value);
        public int CompareTo(string? other) => other == null ? 1 : _value.CompareTo(other);

        public Text Append<T>(T value) => new($"{_value}{value}");

        public static bool TryFrom(string? value, [NotNullWhen(true)] out Text? text) =>
          (text = !string.IsNullOrWhiteSpace(value) ? new Text(value) : null) != null;

        public static Text From(string? value) =>
          TryFrom(value, out var text) ? text : throw new ArgumentOutOfRangeException(nameof(value), "Text cannot be null or whitespace");

        public static bool operator ==(Text? x, Text? y) => EqualityComparer<Text?>.Default.Equals(x, y);
        public static bool operator !=(Text? x, Text? y) => !(x == y);
        public static bool operator <(Text? x, Text? y) => Comparer<Text?>.Default.Compare(x, y) < 0;
        public static bool operator >(Text? x, Text? y) => Comparer<Text?>.Default.Compare(x, y) > 0;
        public static bool operator <=(Text? x, Text? y) => Comparer<Text?>.Default.Compare(x, y) <= 0;
        public static bool operator >=(Text? x, Text? y) => Comparer<Text?>.Default.Compare(x, y) >= 0;

        public static bool operator ==(Text? x, string? y) => EqualityComparer<string?>.Default.Equals(x?._value, y);
        public static bool operator !=(Text? x, string? y) => !(x == y);
        public static bool operator <(Text? x, string? y) => Comparer<string?>.Default.Compare(x?._value, y) < 0;
        public static bool operator >(Text? x, string? y) => Comparer<string?>.Default.Compare(x?._value, y) > 0;
        public static bool operator <=(Text? x, string? y) => Comparer<string?>.Default.Compare(x?._value, y) <= 0;
        public static bool operator >=(Text? x, string? y) => Comparer<string?>.Default.Compare(x?._value, y) >= 0;

        public static implicit operator string(Text value) => value.ToString();
        public static implicit operator Text(byte value) => new(value.ToString());
        public static implicit operator Text(char value) => new(value.ToString());
        public static implicit operator Text(DateTime value) => new(value.ToString());
        public static implicit operator Text(decimal value) => new(value.ToString());
        public static implicit operator Text(double value) => new(value.ToString());
        public static implicit operator Text(float value) => new(value.ToString());
        public static implicit operator Text(int value) => new(value.ToString());
        public static implicit operator Text(long value) => new(value.ToString());
        public static implicit operator Text(ReadOnlySpan<char> value) => new(value.ToString());
        public static implicit operator Text(short value) => new(value.ToString());

        public static explicit operator Text(string value) => From(value);

        public static Text operator +(Text? text, byte value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, char value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, DateTime value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, decimal value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, double value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, float value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, int value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, long value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, ReadOnlySpan<char> value) => text?.Append(value.ToString()) ?? value;
        public static Text operator +(Text? text, short value) => text?.Append(value) ?? value;
        public static Text operator +(Text? text, string? value) => text?.Append(value) ?? From(value);

        class TextJsonConverter : JsonConverter<Text>
        {
            public override Text? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
                TryFrom(reader.GetString(), out var text) ? text : null;

            public override void Write(Utf8JsonWriter writer, Text value, JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString());
        }

        class TextTypeConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) =>
                sourceType == typeof(string);

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) =>
                destinationType == typeof(string);

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) =>
                From(value?.ToString());

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) =>
                destinationType == typeof(string) ? value?.ToString()! : base.ConvertTo(context, culture, value, destinationType);
        }
    }
}