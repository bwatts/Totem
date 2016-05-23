using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Totem.Web.Push
{
	/// <summary>
	/// Indicates a view updated its position on the timeline
	/// </summary>
	public class ViewUpdated : Event
	{
		public ViewUpdated(string key, string etag, JObject diff)
		{
			Key = key;
			ETag = etag;
			Diff = diff;
		}

		public readonly string Key;
		public readonly string ETag;
		public readonly JObject Diff;

		public override Text ToText() => ETag;
	}
}