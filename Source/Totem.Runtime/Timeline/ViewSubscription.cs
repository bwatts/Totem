using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Totem.Runtime.Timeline
{
	/// <summary>
	/// The result of a view subscribing to diffs
	/// </summary>
	public class ViewSubscription
	{
		public ViewSubscription(ViewETag etag, JObject diff = null)
		{
			ETag = etag;
		}

		public readonly ViewETag ETag;
		public readonly JObject Diff;

		public override string ToString() => ETag.ToString();
	}
}