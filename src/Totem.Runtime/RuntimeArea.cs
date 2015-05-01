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
		protected ILog Log { get { return Notion.Traits.Log.Get(this); } }
		protected RuntimeMap Runtime { get { return Notion.Traits.Runtime.Get(this); } }

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
		// IRuntimeArea
		//

		public AreaType AreaType { get; private set; }
		public bool Configured { get; private set; }

		public IConnectable Compose(ILifetimeScope scope)
		{
			return !Configured ? Connection.None : ResolveConnection(scope);
		}

		protected virtual IConnectable ResolveConnection(ILifetimeScope scope)
		{
			return Connection.None;
		}

		//
		// Initialization
		//

		void IPartImportsSatisfiedNotification.OnImportsSatisfied()
		{
			Tags = new Tags();

			ReadAreaType();

			ConfigureArea();
		}

		private void ReadAreaType()
		{
			AreaType = Runtime.GetArea(GetType());
		}

		private void ConfigureArea()
		{
			Configured = !AreaType.HasSettings || ReadSettings();

			if(Configured)
			{
				RegisterArea();
			}
			else
			{
				Log.Debug(
					"Ignorning area {Area}: settings view {SettingsView} not found",
					AreaType.Key,
					AreaType.SettingsView.Key);
			}
		}

		protected virtual bool ReadSettings()
		{
			return true;
		}

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
		[Import]
		public ISettingsDb SettingsDb { get; set; }

		protected TSettings Settings { get; private set; }

		protected virtual bool AllowNullSettings
		{
			get { return false; }
		}

		protected override bool ReadSettings()
		{
			Settings = SettingsDb.ReadViewOrNull<TSettings>();

			return Settings != null || AllowNullSettings;
		}
	}
}