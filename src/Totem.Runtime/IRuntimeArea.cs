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
	/// Describes a set of related objects composed in a Totem runtime
	/// </summary>
	[InheritedExport(typeof(IRuntimeArea))]
	public interface IRuntimeArea : IModule, IWritable, ITaggable
	{
		AreaType AreaType { get; }

		bool HasSettings { get; }

		IConnectable ResolveConnection(ILifetimeScope scope);
	}
}