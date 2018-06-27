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

		public static void Populate(this ContainerExpression expression, IEnumerable<ServiceDescriptor> descriptors)
		{
			var x = expression;

			x.Use<IServiceProvider>().Factory(ctx => new MaestroServiceProvider(ctx.Container));
			x.Use<IServiceScopeFactory>().Factory(ctx => new MaestroServiceScopeFactory(ctx.Container));

			descriptors = descriptors as IReadOnlyCollection<ServiceDescriptor> ?? descriptors.ToList();
			var lookup = descriptors.ToLookup(sd => sd.ServiceType);

			foreach (var descriptor in descriptors)
			{
				var single = lookup[descriptor.ServiceType].Count() == 1;
				x.Register(descriptor, (c, t) => single ? c.Use(t) : c.Add(t));
			}
		}

		private static void Register(this ContainerExpression containerExpression, ServiceDescriptor descriptor, Func<ContainerExpression, Type, IServiceExpression> f)
		{
			var serviceExpression = f(containerExpression, descriptor.ServiceType);

			if (descriptor.ImplementationType != null)
			{
				serviceExpression
					.Type(descriptor.ImplementationType)
					.Lifetime(descriptor.Lifetime);

				return;
			}

			if (descriptor.ImplementationFactory != null)
			{
				serviceExpression
					.Factory(GetFactory(descriptor))
					.Lifetime(descriptor.Lifetime);

				return;
			}

			serviceExpression
				.Instance(descriptor.ImplementationInstance);
		}

		private static void Lifetime<TInstance, TParent>(this IInstanceExpression<TInstance, TParent> expression, ServiceLifetime descriptorLifetime)
		{
			switch (descriptorLifetime)
			{
				case ServiceLifetime.Singleton:
					expression.Singleton();
					break;
				case ServiceLifetime.Scoped:
					expression.Scoped();
					break;
				case ServiceLifetime.Transient:
					expression.Transient();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(descriptorLifetime), descriptorLifetime, null);
			}
		}

		private static Func<Context, object> GetFactory(ServiceDescriptor descriptor)
		{
			return context => descriptor.ImplementationFactory(context.GetService<IServiceProvider>());
		}
	}
}