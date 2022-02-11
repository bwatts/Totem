using System.Collections;

namespace Outermind;

public class MData : MPart, IReadOnlyDictionary<string, string>
{
    public static readonly MData None = new(new Dictionary<string, string>());

    readonly IReadOnlyDictionary<string, string> _data;

    internal MData(IReadOnlyDictionary<string, string> data) =>
        _data = data;

    public override MPartType PartType => MPartType.Data;
    public int Count => _data.Count;
    public string this[string key] => _data[key];
    public IEnumerable<string> Keys => _data.Keys;
    public IEnumerable<string> Values => _data.Values;

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitData(this);

    public MData Rewrite(IReadOnlyDictionary<string, string> data) =>
        data == _data ? this : new(data);

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => _data.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(string key) =>
        _data.ContainsKey(GetPrefixedKey(key));

    public bool TryGetValue(string key, [NotNullWhen(true)] out string? value) =>
        _data.TryGetValue(GetPrefixedKey(key), out value);

    internal static string GetPrefixedKey(string key) =>
        key.Length > 5 && key.StartsWith("data-") ? key : "data-" + key;
}
