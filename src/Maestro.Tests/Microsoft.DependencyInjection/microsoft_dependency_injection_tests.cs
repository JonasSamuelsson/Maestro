using System.Collections.Generic;
using System.Linq;
using Maestro.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Maestro.Tests.Microsoft.DependencyInjection
{
	public class microsoft_dependency_injection_tests
	{
		[Fact]
		public void should_handle_factory_registrations()
		{
			var counter = 1;
			var container = new Container();
			var descriptors = new[] { new ServiceDescriptor(typeof(object), _ => counter++, ServiceLifetime.Transient) };
			container.Populate(descriptors);
			container.GetService<object>().ShouldBe(1);
			container.GetService<object>().ShouldBe(2);
		}

		[Fact]
		public void should_handle_instance_registrations()
		{
			var instance = new object();
			var container = new Container();
			var descriptors = new[] { new ServiceDescriptor(typeof(object), instance) };
			container.Populate(descriptors);
			container.GetService<object>().ShouldBe(instance);
		}

		[Fact]
		public void should_handle_type_registrations()
		{
			var container = new Container();
			var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Transient) };

			container.Populate(descriptors);

			container.GetService<object>().GetType().ShouldBe(typeof(object));
		}

		[Fact]
		public void should_handle_scoped_services()
		{
			var objects = new List<object>();
			var container = new Container();
			var descriptors = new[]
			{
				new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Scoped),
				new ServiceDescriptor(typeof(string), serviceProvider =>
				{
					objects.Add(serviceProvider.GetService<object>());
					objects.Add(serviceProvider.GetService<object>());
					return string.Empty;
				}, ServiceLifetime.Transient)
			};

			container.Populate(descriptors);

			container.GetService<string>();
			container.GetService<string>();

			objects.Count.ShouldBe(4);
			objects.Distinct().Count().ShouldBe(2);
		}

		[Fact]
		public void should_handle_transient_services()
		{
			var container = new Container();
			var descriptors = new[]
			{
				new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Transient)
			};

			container.Populate(descriptors);

			container.GetService<object>().ShouldNotBe(container.GetService<object>());
		}

		[Fact]
		public void should_handle_singleton_services()
		{
			var container = new Container();
			var descriptors = new[]
			{
				new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Singleton)
			};

			container.Populate(descriptors);

			container.GetService<object>().ShouldBe(container.GetService<object>());
		}
	}
}
