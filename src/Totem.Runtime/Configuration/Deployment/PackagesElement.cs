using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Totem.Runtime.Configuration.Deployment
{
	/// <summary>
	/// Configures the packages available to the Totem runtime
	/// </summary>
	public class PackagesElement : ConfigurationElementCollection
	{
		public PackagesElement()
		{
			// Better to set this in the constructor than with ConfigurationCollectionAttribute: http://www.endswithsaurus.com/2008/11/custom-configuration-section.html

			AddElementName = "package";
		}

		protected override string ElementName
		{
			get { return "package"; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new PackageElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((PackageElement) element).Name;
		}

		public IReadOnlyList<string> GetNames()
		{
			// The Totem.Runtime package is always in effect - no need to look for a deployed package

			return this.Cast<PackageElement>()
				.Select(package => package.Name)
				.Where(packageName => packageName != "Totem.Runtime")
				.ToList();
		}
	}
}