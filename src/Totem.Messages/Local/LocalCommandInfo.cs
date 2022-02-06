using System.Diagnostics.CodeAnalysis;
using Totem.Core;

namespace Totem.Local;

public class LocalCommandInfo : CommandInfo, ILocalMessageInfo
{
    static readonly MessageInfoCache<LocalCommandInfo> _cache = new();

    LocalCommandInfo(Type declaredType) : base(declaredType)
    { }

    public static bool TryFrom(Type type, [NotNullWhen(true)] out LocalCommandInfo? info)
    {
        if(type is null)
            throw new ArgumentNullException(nameof(type));

        if(_cache.TryGetValue(type, out info))
        {
            return true;
        }

        if(type is not null && type.IsConcreteClass() && typeof(ILocalCommand).IsAssignableFrom(type))
        {
            info = new LocalCommandInfo(type);

            _cache.Add(info);

            return true;
        }

        info = null;
        return false;
    }

    public static LocalCommandInfo From(Type type)
    {
        if(!TryFrom(type, out var info))
            throw new ArgumentException($"Expected command {type} to be a public, non-abstract, non-or-closed-generic class implementing {typeof(ILocalCommand)}", nameof(type));

        return info;
    }
}
