namespace Totem.Core;

public sealed class ItemKey : IEquatable<ItemKey>, IComparable<ItemKey>
{
    public ItemKey(Type declaredType, Id id)
    {
        DeclaredType = declaredType ?? throw new ArgumentNullException(nameof(declaredType));
        Id = id ?? throw new ArgumentNullException(nameof(id));
    }

    public ItemKey(Type declaredType) : this(declaredType, Id.NewId())
    { }

    public Type DeclaredType { get; }
    public Id Id { get; }

    public bool TypeIsAssignableTo<T>() =>
        typeof(T).IsAssignableFrom(DeclaredType);

    public override string ToString() => $"{DeclaredType.Name}.{Id.ToShortString()}";
    public override int GetHashCode() => HashCode.Combine(DeclaredType, Id);
    public override bool Equals(object? obj) => Equals(obj as ItemKey);

    public bool Equals(ItemKey? other) =>
        other is not null && TypeName == other.TypeName && Id == other.Id;

    public int CompareTo(ItemKey? other)
    {
        if(other is null)
        {
            return 1;
        }

        var typeResult = TypeName.CompareTo(other.TypeName);

        return typeResult != 0 ? typeResult : Id.CompareTo(other.Id);
    }

    string TypeName => DeclaredType.AssemblyQualifiedName ?? DeclaredType.FullName ?? DeclaredType.Name;

    public static bool operator ==(ItemKey? x, ItemKey? y) => EqualityComparer<ItemKey>.Default.Equals(x, y);
    public static bool operator !=(ItemKey? x, ItemKey? y) => !EqualityComparer<ItemKey>.Default.Equals(x, y);
    public static bool operator <(ItemKey? x, ItemKey? y) => Comparer<ItemKey>.Default.Compare(x, y) < 0;
    public static bool operator >(ItemKey? x, ItemKey? y) => Comparer<ItemKey>.Default.Compare(x, y) > 0;
    public static bool operator <=(ItemKey? x, ItemKey? y) => Comparer<ItemKey>.Default.Compare(x, y) <= 0;
    public static bool operator >=(ItemKey? x, ItemKey? y) => Comparer<ItemKey>.Default.Compare(x, y) >= 0;
}
