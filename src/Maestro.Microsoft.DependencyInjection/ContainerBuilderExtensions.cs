using Maestro.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Maestro.Microsoft.DependencyInjection
{
	public static class ContainerBuilderExtensions
	{
		public static void Populate(this IContainerBuilder builder, IEnumerable<ServiceDescriptor> descriptors)
		{
			builder.Add<IServiceProvider>().Factory(ctx => ctx.Scope.ToServiceProvider());
			builder.Add<IServiceScopeFactory>().Factory(ctx => new MaestroServiceScopeFactory(ctx.Container));

			foreach (var descriptor in descriptors)
				builder.Add(descriptor);
		}

		private static void Add(this IContainerBuilder builder, ServiceDescriptor descriptor)
		{
			if (descriptor.ImplementationType != null)
			{
				builder
					.Add(descriptor.ServiceType)
					.Type(descriptor.ImplementationType)
					.Lifetime(descriptor.Lifetime);

				return;
			}

			if (descriptor.ImplementationFactory != null)
			{
				builder
					.Add(descriptor.ServiceType)
					.Factory(GetFactory(descriptor))
					.Lifetime(descriptor.Lifetime);

				return;
			}

			builder
				.Add(descriptor.ServiceType)
				.Instance(descriptor.ImplementationInstance);
		}

		private static void Lifetime<TInstance, TParent>(this IInstanceBuilder<TInstance, TParent> builder, ServiceLifetime descriptorLifetime)
		{
			switch (descriptorLifetime)
			{
				case ServiceLifetime.Singleton:
					builder.Singleton();
					break;
				case ServiceLifetime.Scoped:
					builder.Scoped();
					break;
				case ServiceLifetime.Transient:
					builder.Transient();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(descriptorLifetime), descriptorLifetime, null);
			}
		}

		private static Func<Context, object> GetFactory(ServiceDescriptor descriptor)
		{
			return context => descriptor.ImplementationFactory(context.ToServiceProvider());
		}
	}
}