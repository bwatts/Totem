using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing an element of the Totem runtime
	/// </summary>
	public abstract class RuntimeType : Notion
	{
		protected RuntimeType(RuntimeTypeRef type)
		{
			Package = type.Package;
			DeclaredType = type.DeclaredType;
			Key = type.Key;
		}

		public readonly RuntimePackage Package;
		public readonly Type DeclaredType;
		public readonly RuntimeTypeKey Key;

		public override Text ToText()
		{
			return Key.ToText();
		}

		public bool Is(Type type)
		{
			return type.IsAssignableFrom(DeclaredType);
		}

		public bool Is<T>()
		{
			return Is(typeof(T));
		}

		public bool IsInstance(object runtimeObject)
		{
			return runtimeObject != null && runtimeObject.GetType() == DeclaredType;
		}

		public Expression ConvertToDeclaredType(Expression instance)
		{
			return Expression.Convert(instance, DeclaredType);
		}

		protected void ExpectInstance(object value)
		{
			Expect(IsInstance(value)).IsTrue("Runtime object is not an instance of the specified type", Text.Of(DeclaredType));
		}
	}
}