using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Maestro.Microsoft.DependencyInjection.Tests
{
	public class ServiceRegistrationTests
	{
		[Fact]
		public void should_handle_factory_registrations()
		{
			var counter = 0;
			var descriptors = new[] { new ServiceDescriptor(typeof(object), _ => ++counter, ServiceLifetime.Transient) };

			var container = new Container();
			container.Populate(descriptors);

			new MaestroServiceProvider(container).GetService<object>().ShouldBe(1);
		}

		[Fact]
		public void should_handle_instance_registrations()
		{
			var instance = new object();
			var descriptors = new[] { new ServiceDescriptor(typeof(object), instance) };

			var container = new Container();
			container.Populate(descriptors);

			new MaestroServiceProvider(container).GetService<object>().ShouldBe(instance);
		}

		[Fact]
		public void should_handle_type_registrations()
		{
			var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(TestType), ServiceLifetime.Transient) };

			var container = new Container();
			container.Populate(descriptors);

			new MaestroServiceProvider(container).GetService<object>().GetType().ShouldBe(typeof(TestType));
		}

		[Fact]
		public void should_handle_transient_services()
		{
			var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Transient) };

			var container = new Container();
			container.Populate(descriptors);

			container.GetService<object>().ShouldNotBe(container.GetService<object>());
		}

		[Fact]
		public void should_handle_scoped_services()
		{
			var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Scoped) };

			var objects = new List<object>();
			var container = new Container(x => x.Use<string>().Factory(ctx =>
			{
				objects.Add(ctx.GetService<object>());
				objects.Add(ctx.GetService<object>());
				return string.Empty;
			}));
			container.Populate(descriptors);

			container.GetService<string>();

			objects.Count.ShouldBe(2);
			objects.Distinct().Count().ShouldBe(1);
		}

		[Fact]
		public void should_handle_singleton_services()
		{
			var descriptors = new[] { new ServiceDescriptor(typeof(object), typeof(object), ServiceLifetime.Singleton) };

			var container = new Container();
			container.Populate(descriptors);

			container.GetService<object>().ShouldBe(container.GetService<object>());
		}

		[Fact]
		public void should_handle_multiple_registrations_for_the_same_service_type()
		{
			var descriptors = new[]
			{
				new ServiceDescriptor(typeof(object), "one"),
				new ServiceDescriptor(typeof(object), "two"),
				new ServiceDescriptor(typeof(object), "three")
			};

			var container = new Container();
			container.Populate(descriptors);

			container.GetServices<object>().ShouldBe(new[] { "one", "two", "three" });
		}

		private class TestType { }
	}
}
