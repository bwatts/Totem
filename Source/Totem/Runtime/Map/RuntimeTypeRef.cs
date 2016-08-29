using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A package-qualified reference to a .NET type in the Totem runtime
	/// </summary>
	public sealed class RuntimeTypeRef
	{
		public RuntimeTypeRef(RuntimePackage package, Type declaredType)
		{
			Package = package;
			DeclaredType = declaredType;
			State = new RuntimeState(declaredType);
			Key = RuntimeTypeKey.From(package.RegionKey, ReadName());
		}

		public readonly RuntimePackage Package;
		public readonly Type DeclaredType;
		public readonly RuntimeState State;
		public readonly RuntimeTypeKey Key;

		public override string ToString() => Key.ToString();

		private string ReadName()
		{
			var type = DeclaredType;
			var name = DeclaredType.Name;

			while(type.IsNestedPublic)
			{
				type = type.DeclaringType;

				name = $"{type.Name}.{name}";
			}

			return name;
		}
	}
}