using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Autofac;
using Autofac.Builder;

namespace Totem.Runtime
{
	/// <summary>
	/// Extends area composition with scopes at the runtime, service, and message levels
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class FlowCallScope
	{
		private static readonly string _tag = typeof(FlowCallScope).FullName + ".FlowCall";

		public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InstancePerService<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration)
		{
			return registration.InstancePerMatchingLifetimeScope(_tag);
		}

		public static ILifetimeScope BegiCallScope(this ILifetimeScope baseScope)
		{
			return baseScope.BeginLifetimeScope(_tag);
		}
	}
}