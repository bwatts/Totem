using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Autofac.Features.LightweightAdapters;
using Autofac.Features.Scanning;

namespace Totem.Runtime
{
	/// <summary>
	/// Combines the concepts of <see cref="ContainerBuilder"/> and <see cref="IModule"/> to create a registration syntax
	/// which does not require an external builder variable
	/// </summary>
	/// <remarks>
	/// BuilderModule allows registration through instance methods, which seems cleaner than overriding a
	/// base method and using a builder variable. When done in the constructor, it requires no member variables.
	/// It also makes helper methods easier because the builder doesn't have to be passed around.
	///
	/// Starting in Autofac 2, registration methods are extension methods on ContainerBuilder. The methods below turn the
	/// extension methods into instance methods to achieve an optimal syntax.
	/// </remarks>
	public class BuilderModule : ContainerBuilder, IModule
	{
		void IModule.Configure(IComponentRegistry componentRegistry)
		{
			Configure(componentRegistry);
		}

		/// <summary>
		/// Apply the module to the component registry.
		/// </summary>
		/// <param name="componentRegistry">Component registry to apply configuration to.</param>
		protected virtual void Configure(IComponentRegistry componentRegistry)
		{
			Update(componentRegistry);
		}

		/// <summary>
		/// Register a delegate as a component.
		/// </summary>
		/// <typeparam name="T">The type of the instance.</typeparam>
		/// <param name="delegate">The delegate to register.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(Func<IComponentContext, IEnumerable<Parameter>, T> @delegate)
		{
			return global::Autofac.RegistrationExtensions.Register(this, @delegate);
		}

		/// <summary>
		/// Register a delegate as a component.
		/// </summary>
		/// <typeparam name="T">The type of the instance.</typeparam>
		/// <param name="delegate">The delegate to register.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> Register<T>(Func<IComponentContext, T> @delegate)
		{
			return global::Autofac.RegistrationExtensions.Register(this, @delegate);
		}

		/// <summary>
		/// Adapt all components implementing service TFrom to provide TTo using the provided adapter function.
		/// </summary>
		/// <typeparam name="TFrom">Service type to adapt from.</typeparam>
		/// <typeparam name="TTo">Service type to adapt to. Must not be the same as TFrom.</typeparam>
		/// <param name="adapter">Function adapting TFrom to service TTo, given the context and parameters.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterAdapter<TFrom, TTo>(Func<IComponentContext, IEnumerable<Parameter>, TFrom, TTo> adapter)
		{
			return global::Autofac.RegistrationExtensions.RegisterAdapter(this, adapter);
		}

		/// <summary>
		/// Adapt all components implementing service TFrom to provide TTo using the provided adapter function.
		/// </summary>
		/// <typeparam name="TFrom">Service type to adapt from.</typeparam>
		/// <typeparam name="TTo">Service type to adapt to. Must not be the same as TFrom.</typeparam>
		/// <param name="adapter">Function adapting TFrom to service TTo, given the context.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterAdapter<TFrom, TTo>(Func<IComponentContext, TFrom, TTo> adapter)
		{
			return global::Autofac.RegistrationExtensions.RegisterAdapter(this, adapter);
		}

		/// <summary>
		/// Adapt all components implementing service TFrom to provide TTo using the provided adapter function.
		/// </summary>
		/// <typeparam name="TFrom">Service type to adapt from.</typeparam>
		/// <typeparam name="TTo">Service type to adapt to. Must not be the same as TFrom.</typeparam>
		/// <param name="adapter">Function adapting TFrom to service TTo.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<TTo, LightweightAdapterActivatorData, DynamicRegistrationStyle> RegisterAdapter<TFrom, TTo>(Func<TFrom, TTo> adapter)
		{
			return global::Autofac.RegistrationExtensions.RegisterAdapter(this, adapter);
		}

		/// <summary>
		/// Register the types in an assembly.
		/// </summary>
		/// <param name="assemblies">The assemblies from which to register types.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> RegisterAssemblyTypes(params Assembly[] assemblies)
		{
			return global::Autofac.RegistrationExtensions.RegisterAssemblyTypes(this, assemblies);
		}

		/// <summary>
		/// Add a component to the container.
		/// </summary>
		/// <param name="registration">The component to add.</param>
		public void RegisterComponent(IComponentRegistration registration)
		{
			global::Autofac.RegistrationExtensions.RegisterComponent(this, registration);
		}

		/// <summary>
		/// Register an un-parameterised generic type, e.g. Repository&lt;&gt;. Concrete types will be made as they are requested, e.g. with Resolve&lt;Repository&lt;int&gt;&gt;().
		/// </summary>
		/// <param name="implementor">The open generic implementation type.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle> RegisterGeneric(Type implementor)
		{
			return global::Autofac.RegistrationExtensions.RegisterGeneric(this, implementor);
		}

		/// <summary>
		/// Register an instance as a component.
		/// </summary>
		/// <typeparam name="T">The type of the instance.</typeparam>
		/// <param name="instance">The instance to register.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<T, SimpleActivatorData, SingleRegistrationStyle> RegisterInstance<T>(T instance) where T : class
		{
			return global::Autofac.RegistrationExtensions.RegisterInstance(this, instance);
		}

		/// <summary>
		/// Add a module to the container.
		/// </summary>
		/// <typeparam name="TModule">The module to add.</typeparam>
		public void RegisterModule<TModule>() where TModule : IModule, new()
		{
			global::Autofac.ModuleRegistrationExtensions.RegisterModule<TModule>(this);
		}

		/// <summary>
		/// Add a module to the container.
		/// </summary>
		/// <param name="module">The module to add.</param>
		public void RegisterModule(IModule module)
		{
			global::Autofac.ModuleRegistrationExtensions.RegisterModule(this, module);
		}

		/// <summary>
		/// Add a registration source to the container.
		/// </summary>
		/// <param name="registrationSource">The registration source to add.</param>
		public void RegisterSource(IRegistrationSource registrationSource)
		{
			global::Autofac.RegistrationExtensions.RegisterSource(this, registrationSource);
		}

		/// <summary>
		/// Register a component to be created through reflection.
		/// </summary>
		/// <typeparam name="TImplementor">The type of the component implementation.</typeparam>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<TImplementor, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType<TImplementor>()
		{
			return global::Autofac.RegistrationExtensions.RegisterType<TImplementor>(this);
		}

		/// <summary>
		/// Register a component to be created through reflection.
		/// </summary>
		/// <param name="implementationType">The type of the component implementation.</param>
		/// <returns>Registration builder allowing the registration to be configured.</returns>
		public IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterType(Type implementationType)
		{
			return global::Autofac.RegistrationExtensions.RegisterType(this, implementationType);
		}
	}
}