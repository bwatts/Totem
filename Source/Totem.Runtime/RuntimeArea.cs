﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
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
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    Fields IBindable.Fields => Fields;

    protected Fields Fields { get; } = new Fields();
		protected IClock Clock => Notion.Traits.Clock.Get(this);
		protected ILog Log => Notion.Traits.Log.Get(this);
		protected RuntimeMap Runtime => Notion.Traits.Runtime.Get(this);

		public sealed override string ToString() => ToText();
		public virtual Text ToText() => base.ToString();

		protected static Check<T> Check<T>(T target)
		{
			return Totem.Check.True(target);
		}

		protected static Check<T> CheckNot<T>(T target)
		{
			return Totem.Check.False(target);
		}

		protected static Expect<T> Expect<T>(T target)
		{
			return Totem.Expect.True(target);
		}

		protected static Expect<T> ExpectNot<T>(T target)
		{
			return Totem.Expect.False(target);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void Expect(bool result)
		{
			Totem.Expect.True(result);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void Expect(bool result, Text issue)
		{
			Totem.Expect.True(result, issue);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void ExpectNot(bool result)
		{
			Totem.Expect.False(result);
		}

		[DebuggerHidden, DebuggerStepThrough, DebuggerNonUserCode]
		public static void ExpectNot(bool result, Text issue)
		{
			Totem.Expect.False(result, issue);
		}

		//
		// IRuntimeArea
		//

		public AreaType AreaType { get; private set; }

		public virtual IConnectable Compose(ILifetimeScope scope)
		{
			return Connection.None;
		}

		//
		// Initialization
		//

		void IPartImportsSatisfiedNotification.OnImportsSatisfied()
		{
			ReadAreaType();

			RegisterArea();
		}

		private void ReadAreaType()
		{
			AreaType = Runtime.GetArea(GetType());
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
}