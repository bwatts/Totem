using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime
{
	/// <summary>
	/// Indicates the decorated member is a non-durable part of a durable object
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class TransientAttribute : Attribute
	{

	}
}