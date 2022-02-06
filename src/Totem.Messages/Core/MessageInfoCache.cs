using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace Totem.Core;

internal class MessageInfoCache<T> where T : MessageInfo
{
    readonly ConcurrentDictionary<Type, T> _infoByDeclaredType = new();

    internal bool TryGetValue(Type declaredType, [NotNullWhen(true)] out T? info) =>
        _infoByDeclaredType.TryGetValue(declaredType, out info);

    internal void Add(T info) =>
        _infoByDeclaredType[info.DeclaredType] = info;
}
