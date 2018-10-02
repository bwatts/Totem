using System;
using Totem.Runtime;
using Totem.Runtime.Json;

namespace Totem.Timeline.Area
{
  /// <summary>
  /// A set of .NET types representing instances that persist between usages
  /// </summary>
  public class DurableTypeSet : MapTypeSet<DurableType>
  {
    readonly object _getOrTryDeclareLock = new object();
    readonly AreaKey _areaKey;

    public DurableTypeSet(AreaKey areaKey)
    {
      _areaKey = areaKey;
    }

    protected internal DurableType TryDeclare(Type declaredType)
    {
      if(!IsDurable(declaredType))
      {
        return null;
      }

      var type = new DurableType(new MapTypeInfo(_areaKey, declaredType));

      Declare(type);

      return type;
    }

    protected internal DurableType GetOrTryDeclare(Type declaredType)
    {
      lock(_getOrTryDeclareLock)
      {
        return Get(declaredType, strict: false) ?? TryDeclare(declaredType);
      }
    }

    static bool IsDurable(Type declaredType) =>
      declaredType.IsDefined(typeof(DurableAttribute), inherit: true)
      || (declaredType.IsNestedPublic && IsDurable(declaredType.DeclaringType));
  }
}