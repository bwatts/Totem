using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Totem.Runtime.Hosting
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

    public bool TryResolve(Type type, out object instance)
    {
      return _scope.TryResolve(type, out instance);
    }

    public bool TryResolveNamed(Type type, string name, out object instance)
    {
      return _scope.TryResolveNamed(name, type, out instance);
    }

    public bool TryResolveKeyed(Type type, object key, out object instance)
    {
      return _scope.TryResolveKeyed(key, type, out instance);
    }

    public bool TryResolve<T>(out T instance)
    {
      return _scope.TryResolve(out instance);
    }

    public bool TryResolveNamed<T>(string name, out T instance)
    {
      object untypedInstance;

      var resolved = _scope.TryResolveNamed(name, typeof(T), out untypedInstance);

      instance = resolved ? (T) untypedInstance : default(T);

      return resolved;
    }

    public bool TryResolveKeyed<T>(object key, out T instance)
    {
      object untypedInstance;

      var resolved = _scope.TryResolveKeyed(key, typeof(T), out untypedInstance);

      instance = resolved ? (T) untypedInstance : default(T);

      return resolved;
    }
  }
}