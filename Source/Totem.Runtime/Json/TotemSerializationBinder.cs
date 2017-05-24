using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Totem.Reflection;
using Totem.Runtime.Map;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// Binds durable types to their keys in the runtime
	/// </summary>
	internal sealed class TotemSerializationBinder : DefaultSerializationBinder, IBindable
	{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    Fields Fields { get; } = new Fields();
		RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

    public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			var durableType = Runtime.GetDurable(serializedType, strict: false);

			if(durableType != null)
			{
				assemblyName = null;
				typeName = durableType.Key.ToString();
			}
			else
			{
				base.BindToName(serializedType, out assemblyName, out typeName);
			}
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			var key = RuntimeTypeKey.From(typeName, strict: false);

			if(key != null)
			{
				var durableType = Runtime.GetDurable(key, strict: false);

				if(durableType != null)
				{
					return durableType.DeclaredType;
				}
			}

			return TypeResolver.Resolve(typeName, assemblyName);
		}
	}
}