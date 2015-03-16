using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Totem.Runtime;
using Totem.Runtime.Map;

namespace Totem.Web
{
	/// <summary>
	/// A .NET type representing a part of the Totem web API
	/// </summary>
	public sealed class ApiType : RuntimeType
	{
		private readonly Func<ILifetimeScope, WebApi> _resolve;

		public ApiType(RuntimeTypeRef type, Func<ILifetimeScope, WebApi> resolve = null) : base(type)
		{
			_resolve = resolve;
		}

		public void Register(BuilderModule module)
		{
			if(_resolve == null)
			{
				module.RegisterType(DeclaredType).InstancePerRequest();
			}
			else
			{
				module
					.Register(c => _resolve(c.Resolve<ILifetimeScope>()))
					.As(DeclaredType)
					.InstancePerRequest();
			}
		}

		public WebApi Resolve(ILifetimeScope requestScope)
		{
			return (WebApi) requestScope.Resolve(DeclaredType);
		}
	}
}