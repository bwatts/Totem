using System.Collections;

namespace Outermind;

public class MClasses : MPart, IReadOnlyCollection<string>
{
    public static readonly MClasses None = new(Array.Empty<string>());

    readonly IReadOnlyList<string> _names;

    internal MClasses(IReadOnlyList<string> names) =>
        _names = names;

    public override MPartType PartType => MPartType.Classes;
    public int Count => _names.Count;

    public IEnumerator<string> GetEnumerator() => _names.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() =>
        string.Join(" ", _names);

    protected internal override MPart Accept(MPartVisitor visitor) =>
        visitor.VisitClasses(this);

    public MClasses Rewrite(IReadOnlyList<string> names) =>
        names == _names ? this : new(names);
}
