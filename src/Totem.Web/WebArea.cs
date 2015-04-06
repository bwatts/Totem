using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Totem.IO;
using Totem.Runtime;

namespace Totem.Web
{
	/// <summary>
	/// Registers the elements of the Totem web host
	/// </summary>
	public class WebArea : RuntimeArea<WebAreaView>
	{
		protected override bool AllowNullSettings
		{
			get { return true; }
		}

		protected override void RegisterArea()
		{
			RegisterType<WebHost>().SingleInstance();

			RegisterType<ErrorHandler>().As<IErrorHandler>().SingleInstance();

			Register(c => Settings != null ? Settings.ErrorDetail : ErrorDetail.StackTrace)
			.SingleInstance();
		}

		protected override IConnectable ResolveConnection(ILifetimeScope scope)
		{
			return scope.Resolve<WebHost>();
		}
	}
}