using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Totem;

[JsonConverter(typeof(FilePathJsonConverter))]
[TypeConverter(typeof(FilePathTypeConverter))]
public class FilePath : IEquatable<FilePath>
{
    public static readonly char Separator = '/';

    public FilePath(string root, string key)
    {
        if(string.IsNullOrWhiteSpace(root))
            throw new ArgumentOutOfRangeException(nameof(root));

        if(string.IsNullOrWhiteSpace(key))
            throw new ArgumentOutOfRangeException(nameof(key));

        Root = root;
        Key = key.Trim(Separator);
    }

    public string Root { get; }
    public string Key { get; }

    public override string ToString() =>
        $"{Root}{Separator}{Key}";

    public bool Equals(FilePath? other) =>
        other is FilePath path && Root == path.Root && Key == path.Key;

    public override bool Equals(object? obj) =>
        Equals(obj as FilePath);

    public override int GetHashCode() =>
        HashCode.Combine(Root, Key);

    public int CompareTo(FilePath? other)
    {
        if(other is null)
        {
            return 1;
        }

        return Root.CompareTo(other.Root) switch
        {
            > 0 => 1,
            < 0 => -1,
            _ => Key.CompareTo(other.Key)
        };
    }

    public static bool operator ==(FilePath? x, FilePath? y) => EqualityComparer<FilePath>.Default.Equals(x, y);
    public static bool operator !=(FilePath? x, FilePath? y) => !EqualityComparer<FilePath>.Default.Equals(x, y);

    public static bool TryFrom(string? value, [NotNullWhen(true)] out FilePath? key)
    {
        if(!string.IsNullOrWhiteSpace(value))
        {
            var separatorIndex = value.IndexOf(Separator);

            if(separatorIndex != -1 && separatorIndex != value.Length - 1)
            {
                var root = value[..separatorIndex];
                var path = value[(separatorIndex + 1)..];

                key = new FilePath(root, path);
                return true;
            }
        }

        key = null;
        return false;
    }

    public static FilePath From(string value)
    {
        if(!TryFrom(value, out var path))
            throw new ArgumentException($"Expected file path in the format [root]/[key...]", nameof(value));

        return path;
    }

    class FilePathJsonConverter : JsonConverter<FilePath>
    {
        public override FilePath? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            TryFrom(reader.GetString(), out var text) ? text : null;

        public override void Write(Utf8JsonWriter writer, FilePath value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString());
    }

    class FilePathTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
            sourceType == typeof(string) || sourceType == typeof(Guid);

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType) =>
            destinationType == typeof(string) || destinationType == typeof(Guid);

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) =>
            value is string key ? From(key) : base.ConvertFrom(context, culture, value);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType) =>
            destinationType == typeof(string) && value is FilePath filePath
                ? filePath.ToString()
                : base.ConvertTo(context, culture, value, destinationType);
    }
}
