using System;
using System.Collections.Generic;
using System.Linq;

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

		public ApiType GetApi(RuntimeTypeKey key, bool strict = true)
		{
			var api = Packages
				.Select(package => package.GetApi(key, strict: false))
				.FirstOrDefault(packageApi => packageApi != null);

			Expect(strict && api == null).IsFalse("Failed to resolve API", key.ToText());

			return api;
		}

		public ApiType GetApi(Type declaredType, bool strict = true)
		{
			var api = Packages
				.Select(package => package.GetApi(declaredType, strict: false))
				.FirstOrDefault(packageApi => packageApi != null);

			Expect(strict && api == null).IsFalse("Failed to resolve API", Text.Of(declaredType));

			return api;
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
	}
}