using System;
using System.Collections.Generic;
using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			container.Configure(x =>
			{
				x.For<IServiceProvider>().Use.Instance(new MaestroServiceProvider(container));

				foreach (var descriptor in descriptors)
				{
					x.Register(descriptor);
				}
			});
		}

		private static void Register(this IContainerExpression expression, ServiceDescriptor descriptor)
		{
			if (descriptor.ImplementationType != null)
			{
				expression.For(descriptor.ServiceType)
					 .Use.Type(descriptor.ImplementationType)
					 .Lifetime.Use(descriptor.Lifetime);

				return;
			}

			if (descriptor.ImplementationFactory != null)
			{
				expression.For(descriptor.ServiceType)
					 .Use.Factory(GetFactory(descriptor))
					 .Lifetime.Use(descriptor.Lifetime);

				return;
			}

			expression.For(descriptor.ServiceType)
				 .Use.Instance(descriptor.ImplementationInstance);
		}

		private static void Use<T>(this ILifetimeExpression<T> expression, ServiceLifetime descriptorLifetime)
		{
			switch (descriptorLifetime)
			{
				case ServiceLifetime.Singleton:
					expression.Singleton();
					break;
				case ServiceLifetime.Scoped:
					expression.Context();
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

		private class Marker { }
	}
}