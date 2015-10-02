using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.Reflection
{
	/// <summary>
	/// Extends types to get their chains of inheritance
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class InheritanceChain
	{
		//
		// To
		//

		public static IEnumerable<Type> GetInheritanceChainTo(this Type type, Type targetType, bool requireTargetType = false, bool includeType = false, bool includeTargetType = false)
		{
			if(includeType)
			{
				yield return type;
			}

			var currentType = type.BaseType;

			while(currentType != null && currentType != targetType)
			{
				yield return currentType;

				currentType = currentType.BaseType;
			}

			if(currentType == null)
			{
				Expect.That(requireTargetType).IsFalse(Text.Of("Target type {0} is not in chain of {1}", targetType, type));
			}
			else
			{
				if(includeTargetType)
				{
					yield return targetType;
				}
			}
		}

		public static IEnumerable<Type> GetInheritanceChainTo<T>(this Type type, bool requireTargetType = false, bool includeType = false, bool includeTargetType = false)
		{
			return type.GetInheritanceChainTo(typeof(T), requireTargetType, includeType, includeTargetType);
		}

		public static IEnumerable<Type> GetInheritanceChainToObject(this Type type, bool includeType = false, bool includeObject = false)
		{
			return type.GetInheritanceChainTo<object>(includeType: includeType, includeTargetType: includeObject);
		}

		//
		// From
		//

		public static IEnumerable<Type> GetInheritanceChainFrom(this Type type, Type targetType, bool requireTargetType = false, bool includeType = false, bool includeTargetType = false)
		{
			return type.GetInheritanceChainTo(targetType, requireTargetType, includeType, includeTargetType).Reverse();
		}

		public static IEnumerable<Type> GetInheritanceChainFrom<T>(this Type type, bool requireTargetType = false, bool includeType = false, bool includeTargetType = false)
		{
			return type.GetInheritanceChainTo<T>(requireTargetType, includeType, includeTargetType).Reverse();
		}

		public static IEnumerable<Type> GetInheritanceChainFromObject(this Type type, bool includeType = false, bool includeObject = false)
		{
			return type.GetInheritanceChainToObject(includeType, includeObject).Reverse();
		}
	}
}