using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Autofac;
using Autofac.Builder;

namespace Totem.Runtime
{
	/// <summary>
	/// Extends area composition with a scope at the call level
	/// </summary>
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class CallScope
	{
		private static readonly string _tag = typeof(CallScope).FullName + ".Call";

		public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> InstancePerCall<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration)
		{
			return registration.InstancePerMatchingLifetimeScope(_tag);
		}

		public static ILifetimeScope BeginCallScope(this ILifetimeScope baseScope)
		{
			return baseScope.BeginLifetimeScope(_tag);
		}
	}
}