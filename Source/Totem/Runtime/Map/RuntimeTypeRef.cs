using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A package-qualified reference to a .NET type in the Totem runtime
	/// </summary>
	public sealed class RuntimeTypeRef : Notion
	{
		public RuntimeTypeRef(RuntimePackage package, Type declaredType, RuntimeState state)
		{
			Package = package;
			DeclaredType = declaredType;
			State = state;
			Key = RuntimeTypeKey.From(package.RegionKey, declaredType.Name);
		}

		public readonly RuntimePackage Package;
		public readonly Type DeclaredType;
		public readonly RuntimeState State;
		public readonly RuntimeTypeKey Key;

		public override Text ToText()
		{
			return Key.ToText();
		}
	}
}