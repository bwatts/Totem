using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Totem.Runtime
{
	/// <summary>
	/// The source of dependencies during runtime work
	/// </summary>
	public sealed class DependencySource : IDependencySource
	{
		private readonly ILifetimeScope _scope;

		public DependencySource(ILifetimeScope scope)
		{
			_scope = scope;
		}

		public object Resolve(Type type)
		{
			return _scope.Resolve(type);
		}

		public object ResolveNamed(Type type, string name)
		{
			return _scope.ResolveNamed(name, type);
		}

		public object ResolveKeyed(Type type, object key)
		{
			return _scope.ResolveKeyed(key, type);
		}

		public T Resolve<T>()
		{
			return _scope.Resolve<T>();
		}

		public T ResolveNamed<T>(string name)
		{
			return _scope.ResolveNamed<T>(name);
		}

		public T ResolveKeyed<T>(object key)
		{
			return _scope.ResolveKeyed<T>(key);
		}
	}
}