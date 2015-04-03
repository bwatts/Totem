using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Http;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A .NET type representing a Totem web API
	/// </summary>
	public sealed class ApiType : RuntimeType
	{
		private readonly Func<IDependencySource, IWebApi> _resolve;

		internal ApiType(RuntimeTypeRef type, HttpResource resource, Func<IDependencySource, IWebApi> resolve) : base(type)
		{
			Resource = resource;
			_resolve = resolve;
		}

		public readonly HttpResource Resource;

		public IWebApi Resolve(IDependencySource source)
		{
			return _resolve(source);
		}
	}
}