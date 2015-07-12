using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;
using Totem.Reflection;
using Totem.Runtime.Map;

namespace Totem.Runtime.Json
{
	/// <summary>
	/// Resolves type names by working around a minor bug in JSON.NET
	/// </summary>
	internal sealed class TotemSerializationBinder : DefaultSerializationBinder, ITaggable
	{
		internal TotemSerializationBinder()
		{
			Tags = new Tags();
		}

		Tags ITaggable.Tags { get { return Tags; } }
		protected Tags Tags { get; private set; }
		protected RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

		public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
		{
			var eventType = Runtime.GetEvent(serializedType, strict: false);

			if(eventType != null)
			{
				assemblyName = null;
				typeName = eventType.Key.ToString();
			}
			else
			{
				base.BindToName(serializedType, out assemblyName, out typeName);
			}
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			var eventTypeKey = RuntimeTypeKey.From(typeName, strict: false);

			return eventTypeKey != null
				? Runtime.GetEvent(eventTypeKey).DeclaredType
				: TypeResolver.Resolve(typeName, assemblyName);
		}
	}
}