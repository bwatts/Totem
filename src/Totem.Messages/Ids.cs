using System.Collections;

namespace Totem;

public class Ids : IReadOnlySet<Id>
{
    readonly HashSet<Id> _ids;

    public Ids(IEnumerable<Id> ids)
    {
        if(ids is null)
            throw new ArgumentNullException(nameof(ids));

        _ids = ids.ToHashSet();
    }

    public int Count => _ids.Count;

    public bool Contains(Id item) => _ids.Contains(item);
    public IEnumerator<Id> GetEnumerator() => _ids.GetEnumerator();
    public bool IsProperSubsetOf(IEnumerable<Id> other) => _ids.IsProperSubsetOf(other);
    public bool IsProperSupersetOf(IEnumerable<Id> other) => _ids.IsProperSupersetOf(other);
    public bool IsSubsetOf(IEnumerable<Id> other) => _ids.IsSubsetOf(other);
    public bool IsSupersetOf(IEnumerable<Id> other) => _ids.IsSupersetOf(other);
    public bool Overlaps(IEnumerable<Id> other) => _ids.Overlaps(other);
    public bool SetEquals(IEnumerable<Id> other) => _ids.SetEquals(other);
    IEnumerator IEnumerable.GetEnumerator() => _ids.GetEnumerator();

    public static readonly Ids Empty = new(Enumerable.Empty<Id>());
}
