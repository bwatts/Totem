using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Totem.Reflection
{
	/// <summary>
	/// Extension methods to check whether types are anonymous
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class AnonymousTypes
	{
		public static bool IsAnonymous(this Type type)
		{
			// http://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous
			//
			// Specifically: http://stackoverflow.com/a/11472757/37815

			var definition = type.IsGenericType ? type.GetGenericTypeDefinition() : null;

			return definition != null
				&& definition.IsClass
				&& definition.IsSealed
				&& definition.Attributes.HasFlag(TypeAttributes.NotPublic)
				&& definition.IsDefined(typeof(CompilerGeneratedAttribute));
		}
	}
}