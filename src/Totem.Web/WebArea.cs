using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;
using Nancy.Owin;
using Totem.Http;
using Totem.Runtime;
using Totem.Runtime.Map;
using Totem.Web.Configuration;

namespace Totem.Web
{
	/// <summary>
	/// Registers the elements of Totem web APIs
	/// </summary>
	[ConfigurationSection("totem.web")]
	public class WebArea : RuntimeArea<WebSection>
	{
		protected override void RegisterArea()
		{
			RegisterHost();

			RegisterApiTypes();
		}

		public override IConnectable ResolveConnectionOrNull(ILifetimeScope scope)
		{
			return scope.Resolve<WebHost>();
		}

		private void RegisterHost()
		{
			Register(c => new WebHost(Section.HostBinding, c.Resolve<IEnumerable<IWebApplication>>().ToList()))
			.SingleInstance();

			Register(c => new ApiApplication(
				c.Resolve<CompositionRoot>(),
				new NancyOptions(),
				Section.HostBinding.Resource.Then(Section.ApiRoot),
				c.Resolve<ApiTypeSet>(),
				Type))
			.As<IWebApplication>()
			.SingleInstance();

			Register(c => new PushApplication(
				Section.HostBinding.Resource.Then(Section.PushRoot),
				c.Resolve<ILifetimeScope>()))
			.As<IWebApplication>()
			.SingleInstance();

			RegisterType<ErrorHandler>().As<IErrorHandler>().SingleInstance();
		}

		private void RegisterApiTypes()
		{
			var apiTypes = ReadApiTypes();

			RegisterInstance(apiTypes);

			foreach(var api in apiTypes)
			{
				api.Register(this);
			}
		}

		private ApiTypeSet ReadApiTypes()
		{
			return new ApiTypeSet(
				from region in Runtime.Regions
				from package in region.Packages
				from type in package.Assembly.GetTypes()
				where type.IsClass && !type.IsAbstract && typeof(WebApi).IsAssignableFrom(type)
				select ReadApi(package, type));
		}

		private static ApiType ReadApi(RuntimePackage package, Type type)
		{
			return new ApiType(new RuntimeTypeRef(package, type, new RuntimeState(type)), ReadResolve(type));
		}

		private static Func<ILifetimeScope, WebApi> ReadResolve(Type type)
		{
			var attribute = type.GetCustomAttribute<ResolveMethodAttribute>();

			if(attribute == null || attribute.MethodName == "")
			{
				return null;
			}

			var method = type.GetMethod(attribute.MethodName, BindingFlags.Public | BindingFlags.Static);

			Expect(method).IsNotNull("Missing resolve method: " + attribute.MethodName);

			var parameters = method.GetParameters();

			Expect(parameters.Length != 1
				|| parameters[0].ParameterType != typeof(ILifetimeScope)
				|| method.ReturnType != typeof(WebApi))
				.IsTrue("Invalid resolve method signature: " + Text.Of(method));

			var scopeParameter = Expression.Parameter(typeof(ILifetimeScope), "scope");

			var call = Expression.Call(method, scopeParameter);

			var callLambda = Expression.Lambda<Func<ILifetimeScope, WebApi>>(call, scopeParameter);

			return callLambda.Compile();
		}
	}
}