using System;

namespace Totem.Map;

public abstract class MapType : ITypeKeyed
{
    internal MapType(Type declaredType) =>
        DeclaredType = declaredType;

    public Type DeclaredType { get; }

    Type ITypeKeyed.TypeKey => DeclaredType;

    public override string ToString() =>
        DeclaredType.ToString();

    public bool IsAssignableTo(Type type) =>
        type?.IsAssignableFrom(DeclaredType) ?? throw new ArgumentNullException(nameof(type));

    public bool IsAssignableTo<T>() =>
        IsAssignableTo(typeof(T));
}
