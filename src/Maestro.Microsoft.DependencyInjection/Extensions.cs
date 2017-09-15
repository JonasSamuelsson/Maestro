using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Maestro.Microsoft.DependencyInjection
{
	public static class Extensions
	{
		/// <summary>
		/// Populates the container using the specified service descriptors.
		/// </summary>
		/// <remarks>
		/// This method should only be called once per container.
		/// </remarks>
		/// <param name="container">The container.</param>
		/// <param name="descriptors">The service descriptors.</param>
		public static void Populate(this IContainer container, IEnumerable<ServiceDescriptor> descriptors)
		{
			container.Configure(x => x.Populate(descriptors));
		}

		public static void Populate(this ContainerBuilder builder, IEnumerable<ServiceDescriptor> descriptors)
		{
			builder.Configure(x => x.Populate(descriptors));
		}

		public static void Populate(this IContainerExpression expression, IEnumerable<ServiceDescriptor> descriptors)
		{
			var x = expression;

			x.Config.GetServicesOrder = GetServicesOrder.Ordered;

			x.For<IServiceProvider>().Use.Factory(ctx => new MaestroServiceProvider(ctx.Container));
			x.For<IServiceScopeFactory>().Use.Factory(ctx => new MaestroServiceScopeFactory(ctx.Container));

			descriptors = descriptors as IReadOnlyCollection<ServiceDescriptor> ?? descriptors.ToList();
			var lookup = descriptors.ToLookup(sd => sd.ServiceType);

			foreach (var descriptor in descriptors)
			{
				var single = lookup[descriptor.ServiceType].Count() == 1;
				x.Register(descriptor, se => single ? se.Use : se.Add);
			}
		}

		private static void Register(this IContainerExpression containerExpression, ServiceDescriptor descriptor, Func<IServiceExpression, IInstanceKindSelector> f)
		{
			var instanceKindSelector = f(containerExpression.For(descriptor.ServiceType));

			if (descriptor.ImplementationType != null)
			{
				instanceKindSelector
					.Type(descriptor.ImplementationType)
					.Lifetime.Use(descriptor.Lifetime);

				return;
			}

			if (descriptor.ImplementationFactory != null)
			{
				instanceKindSelector
					.Factory(GetFactory(descriptor))
					.Lifetime.Use(descriptor.Lifetime);

				return;
			}

			instanceKindSelector
				.Instance(descriptor.ImplementationInstance);
		}

		private static void Use<T>(this ILifetimeSelector<T> expression, ServiceLifetime descriptorLifetime)
		{
			switch (descriptorLifetime)
			{
				case ServiceLifetime.Singleton:
					expression.Singleton();
					break;
				case ServiceLifetime.Scoped:
					expression.ContainerScoped();
					break;
				case ServiceLifetime.Transient:
					expression.Transient();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(descriptorLifetime), descriptorLifetime, null);
			}
		}

		private static Func<IContext, object> GetFactory(ServiceDescriptor descriptor)
		{
			return context => descriptor.ImplementationFactory(context.GetService<IServiceProvider>());
		}
	}
}