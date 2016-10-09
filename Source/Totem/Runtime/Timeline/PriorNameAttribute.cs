using System;
using System.Collections.Generic;
using System.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// Indicates the decorated <see cref="Flow"/> was renamed from a prior type
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
	public sealed class PriorNameAttribute : Attribute
	{
    public PriorNameAttribute(string priorName)
    {
      PriorName = priorName;
    }

    public readonly string PriorName;
	}
}