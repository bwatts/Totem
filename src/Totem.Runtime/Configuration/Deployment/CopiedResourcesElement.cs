using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Totem.IO;

namespace Totem.Runtime.Configuration.Deployment
{
	/// <summary>
	/// Configures the resources to copy during deployment of the Totem runtime
	/// </summary>
	public class CopiedResourcesElement : ConfigurationElementCollection
	{
		public CopiedResourcesElement()
		{
			AddElementName = "copy";
		}

		protected override string ElementName
		{
			get { return "copy"; }
		}

		protected override ConfigurationElement CreateNewElement()
		{
			return new CopyElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CopyElement) element).Resource;
		}

		public IReadOnlyList<IOResource> GetResources()
		{
			return this.Cast<CopyElement>().Select(copy => copy.Resource).ToList();
		}
	}
}