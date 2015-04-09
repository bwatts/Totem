using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Nancy;
using Nancy.Serialization.JsonNet;
using Newtonsoft.Json;
using Totem.IO;
using Totem.Runtime;
using Totem.Runtime.Json;

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

			// Override all JSON handling in Nancy with our custom settings. For some reason, IBodyDeserializer does not need to be overridden - it just
			// works when referencing the JSON.NET implementation. Solar flares.

			Register(c =>
			{
				var settings = new TotemSerializerSettings
				{
					CamelCaseProperties = true
				};

				return new JsonNetSerializer(JsonSerializer.Create(settings));
			})
			.As<ISerializer>()
			.SingleInstance();
		}

		protected override IConnectable ResolveConnection(ILifetimeScope scope)
		{
			return scope.Resolve<WebHost>();
		}
	}
}