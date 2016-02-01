using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Totem.Runtime.Map.Timeline;

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

		public bool Is(RuntimeType type)
		{
			return Is(type.DeclaredType);
		}

		public bool IsTypeOf(object instance)
		{
			return instance != null && instance.GetType() == DeclaredType;
		}

		public bool CanAssign(object instance)
		{
			return instance != null && Is(instance.GetType());
		}

		public bool CanAssign(RuntimeType type)
		{
			return Is(type.DeclaredType);
		}

		public Expression ConvertToDeclaredType(Expression instance)
		{
			return Expression.Convert(instance, DeclaredType);
		}

		protected void ExpectInstance(object value)
		{
			Expect(IsTypeOf(value), "Runtime object is not an instance of the specified type");
		}
	}
}