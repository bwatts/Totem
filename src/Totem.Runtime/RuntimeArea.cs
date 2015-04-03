using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Autofac;
using Autofac.Core;
using Totem.IO;
using Totem.Runtime.Map;

namespace Totem.Runtime
{
	/// <summary>
	/// A set of related objects available for hosting by a runtime
	/// </summary>
	public abstract class RuntimeArea : BuilderModule, IRuntimeArea, IPartImportsSatisfiedNotification
	{
		Tags ITaggable.Tags { get { return Tags; } }
		protected Tags Tags { get; private set; }
		protected IClock Clock { get { return Notion.Traits.Clock.Get(this); } }
		protected RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

		[Import]
		protected IViewStore Views { get; set; }

		public AreaType AreaType { get; private set; }

		public virtual bool HasSettings { get { return false; } }

		public virtual IConnectable ResolveConnection(ILifetimeScope scope)
		{
			return Connection.None;
		}

		public sealed override string ToString()
		{
			return ToText();
		}

		public virtual Text ToText()
		{
			return base.ToString();
		}

		protected static IExpect<T> Expect<T>(T value)
		{
			return Totem.Expect.That(value);
		}

		//
		// Initialization
		//

		void IPartImportsSatisfiedNotification.OnImportsSatisfied()
		{
			Tags = new Tags();

			ReadAreaType();

			if(AreaType.HasSettings)
			{
				ReadSettings();
			}

			RegisterArea();
		}

		private void ReadAreaType()
		{
			AreaType = Runtime.GetArea(GetType());
		}

		protected virtual void ReadSettings()
		{}

		protected abstract void RegisterArea();

		// Clean up derived API (too much?)

		protected sealed override void Configure(IComponentRegistry componentRegistry)
		{
			base.Configure(componentRegistry);
		}

		public sealed override void RegisterCallback(Action<IComponentRegistry> configurationCallback)
		{
			base.RegisterCallback(configurationCallback);
		}

		//
		// Path expansion
		//

		public FolderLink Expand(FolderResource folder)
		{
			return Runtime.Deployment.Expand(folder);
		}

		public FileLink Expand(FileResource file)
		{
			return Runtime.Deployment.Expand(file);
		}

		public FolderLink ExpandInData(FolderResource folder)
		{
			return Runtime.Deployment.ExpandInData(folder);
		}

		public FileLink ExpandInData(FileResource file)
		{
			return Runtime.Deployment.ExpandInData(file);
		}
	}

	/// <summary>
	/// A set of related objects and settings available for hosting by a runtime
	/// </summary>
	/// <typeparam name="TSettings">The type of view providing the area's settings</typeparam>
	public abstract class RuntimeArea<TSettings> : RuntimeArea where TSettings : View
	{
		protected TSettings Settings { get; private set; }

		protected override void ReadSettings()
		{
			Settings = AreaType.ReadSettings<TSettings>(Views, strict: false);
		}
	}
}