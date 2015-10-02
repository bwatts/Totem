using System;
using System.Collections.Generic;
using System.Linq;
using Totem.Runtime.Map.Timeline;

namespace Totem.Runtime.Map
{
	/// <summary>
	/// A set of related areas in a Totem runtime
	/// </summary>
	public sealed class RuntimeRegion : Notion
	{
		public RuntimeRegion(RuntimeRegionKey key, RuntimePackageSet packages)
		{
			Key = key;
			Packages = packages;
		}

		public RuntimeRegionKey Key { get; private set; }
		public RuntimePackageSet Packages { get; private set; }

		public override Text ToText()
		{
			return Key.ToText();
		}

		public RuntimePackage GetPackage(string name, bool strict = true)
		{
			return Packages.Get(name, strict);
		}

		public AreaType GetArea(RuntimeTypeKey key, bool strict = true)
		{
			var area = Packages
				.Select(package => package.GetArea(key, strict: false))
				.FirstOrDefault(packageArea => packageArea != null);

			Expect(strict && area == null).IsFalse("Failed to resolve area", key.ToText());

			return area;
		}

		public AreaType GetArea(Type declaredType, bool strict = true)
		{
			var area = Packages
				.Select(package => package.GetArea(declaredType, strict: false))
				.FirstOrDefault(packageArea => packageArea != null);

			Expect(strict && area == null).IsFalse("Failed to resolve area", Text.Of(declaredType));

			return area;
		}

		public ViewType GetView(RuntimeTypeKey key, bool strict = true)
		{
			var view = Packages
				.Select(package => package.GetView(key, strict: false))
				.FirstOrDefault(packageView => packageView != null);

			Expect(strict && view == null).IsFalse("Failed to resolve view", key.ToText());

			return view;
		}

		public ViewType GetView(Type declaredType, bool strict = true)
		{
			var view = Packages
				.Select(package => package.GetView(declaredType, strict: false))
				.FirstOrDefault(packageView => packageView != null);

			Expect(strict && view == null).IsFalse("Failed to resolve view", Text.Of(declaredType));

			return view;
		}

		public FlowType GetFlow(RuntimeTypeKey key, bool strict = true)
		{
			var flow = Packages
				.Select(package => package.GetFlow(key, strict: false))
				.FirstOrDefault(packageFlow => packageFlow != null);

			Expect(strict && flow == null).IsFalse("Failed to resolve flow", key.ToText());

			return flow;
		}

		public FlowType GetFlow(Type declaredType, bool strict = true)
		{
			var flow = Packages
				.Select(package => package.GetFlow(declaredType, strict: false))
				.FirstOrDefault(packageFlow => packageFlow != null);

			Expect(strict && flow == null).IsFalse("Failed to resolve flow", Text.Of(declaredType));

			return flow;
		}

		public EventType GetEvent(RuntimeTypeKey key, bool strict = true)
		{
			var e = Packages
				.Select(package => package.GetEvent(key, strict: false))
				.FirstOrDefault(packageEvent => packageEvent != null);

			Expect(strict && e == null).IsFalse("Failed to resolve event", key.ToText());

			return e;
		}

		public EventType GetEvent(Type declaredType, bool strict = true)
		{
			var e = Packages
				.Select(package => package.GetEvent(declaredType, strict: false))
				.FirstOrDefault(packageEvent => packageEvent != null);

			Expect(strict && e == null).IsFalse("Failed to resolve event", Text.Of(declaredType));

			return e;
		}

		public WebApiType GetWebApi(RuntimeTypeKey key, bool strict = true)
		{
			var e = Packages
				.Select(package => package.GetWebApi(key, strict: false))
				.FirstOrDefault(packageWebApi => packageWebApi != null);

			Expect(strict && e == null).IsFalse("Failed to resolve web API", key.ToText());

			return e;
		}

		public WebApiType GetWebApi(Type declaredType, bool strict = true)
		{
			var e = Packages
				.Select(package => package.GetWebApi(declaredType, strict: false))
				.FirstOrDefault(packageWebApi => packageWebApi != null);

			Expect(strict && e == null).IsFalse("Failed to resolve web API", Text.Of(declaredType));

			return e;
		}
	}
}