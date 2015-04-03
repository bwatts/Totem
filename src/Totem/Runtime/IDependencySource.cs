using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes the source of dependencies during runtime work
	/// </summary>
	public interface IDependencySource
	{
		object Resolve(Type type);

		object ResolveNamed(Type type, string name);

		object ResolveKeyed(Type type, object key);

		T Resolve<T>();

		T ResolveNamed<T>(string name);

		T ResolveKeyed<T>(object key);
	}
}