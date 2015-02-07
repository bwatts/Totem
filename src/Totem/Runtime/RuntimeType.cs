using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// A .NET type representing an element of the Totem runtime
	/// </summary>
	public abstract class RuntimeType : Notion
	{
		protected RuntimeType(RuntimePackage package, Type declaredType)
		{
			Package = package;
			DeclaredType = declaredType;

			Key = RuntimeTypeKey.From(package.RegionKey, declaredType.Name);
		}

		public RuntimePackage Package { get; private set; }
		public RuntimeTypeKey Key { get; private set; }
		public Type DeclaredType { get; private set; }

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

		protected void ExpectIsInstance(object runtimeObject)
		{
			Expect(IsInstance(runtimeObject)).IsTrue("Runtime object is not an instance of the specified type", Text.Of(DeclaredType));
		}
	}
}