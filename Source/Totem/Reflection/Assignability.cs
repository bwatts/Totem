using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Totem.Reflection
{
	/// <summary>
	/// Extends <see cref="System.Type"/> to check for assignability of generic types
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class Assignability
	{
		public static Type GetAssignableGenericType(this Type openGenericType, Type closedGenericType, bool strict = true)
		{
			var assignableGenericType = closedGenericType
				.GetInheritanceChainToObject(includeType: true)
				.FirstOrDefault(ancestorType => ancestorType.IsGenericType && ancestorType.GetGenericTypeDefinition() == openGenericType);

			Expect.False(strict && assignableGenericType == null, Text.Of(
				"No generic type in the inheritance chain of {0} is assignable to {1}",
				closedGenericType,
				openGenericType));

			return assignableGenericType;
		}

		public static bool IsAssignableFromGeneric(this Type openGenericType, Type closedGenericType)
		{
			return openGenericType.GetAssignableGenericType(closedGenericType, strict: false) != null;
		}

		public static bool IsAssignableNull(this Type type)
		{
			return type.IsClass
				|| type.IsInterface
				|| (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		public static object GetDefaultValue(this Type type)
		{
			return type.IsValueType ? Activator.CreateInstance(type) : null;
		}
	}
}