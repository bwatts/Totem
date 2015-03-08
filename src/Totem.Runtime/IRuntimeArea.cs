using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Autofac;
using Autofac.Core;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// Describes a set of related objects in a Totem runtime
	/// </summary>
	[InheritedExport(typeof(IRuntimeArea))]
	public interface IRuntimeArea : IModule, IWritable, ITaggable
	{
		AreaType Type { get; }

		IConnectable ResolveConnectionOrNull(ILifetimeScope scope);
	}
}